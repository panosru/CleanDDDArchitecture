// Local orchestration with .NET Aspire.
//
//   dotnet run --project Hosts/AppHost                          # monolith (default)
//   dotnet run --project Hosts/AppHost -- --mode microservices  # one service per domain + gateway
//
// Both modes run the same domain code; only the hosts differ. Integration events between
// contexts travel in-process in the monolith and over Kafka between the services. Every service receives its
// settings under the names it already reads (ConnectionStrings:PGSQLConnection, :kafka,
// :kurrentdb, EmailSettings:*), so nothing in the services knows about Aspire beyond
// AddServiceDefaults(), which sends traces, metrics and logs to the Aspire dashboard.

var builder = DistributedApplication.CreateBuilder(args);

var microservices = string.Equals(builder.Configuration["mode"], "microservices", StringComparison.OrdinalIgnoreCase);

// ── Infrastructure ───────────────────────────────────────────────────────────

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("cleanddd-postgres-data");

var kafka = builder.AddKafka("kafka");

// KurrentDB (formerly EventStoreDB), reached over gRPC on its HTTP port.
var kurrentDb = builder.AddContainer("kurrentdb", "kurrentplatform/kurrentdb", "26.1.2")
    .WithEnvironment("KURRENTDB_CLUSTER_SIZE", "1")
    .WithEnvironment("KURRENTDB_RUN_PROJECTIONS", "None")
    .WithEnvironment("KURRENTDB_INSECURE", "true")
    .WithVolume("cleanddd-kurrentdb-data", "/var/lib/kurrentdb")
    .WithHttpEndpoint(targetPort: 2113, name: "http")
    .WithHttpHealthCheck("/health/live", 204, "http");

var kurrentDbConnection = ReferenceExpression.Create(
    $"kurrentdb://admin:changeit@{kurrentDb.GetEndpoint("http").Property(EndpointProperty.HostAndPort)}?tls=false");

var mailpit = builder.AddContainer("mailpit", "axllent/mailpit")
    .WithHttpEndpoint(targetPort: 8025, name: "ui")
    .WithEndpoint(targetPort: 1025, name: "smtp", scheme: "tcp");

var smtp = mailpit.GetEndpoint("smtp");

if (microservices)
{
    // ── Microservices: one host per domain behind the YARP gateway ──────────
    // These hosts have no launch profile, so their HTTP endpoints are declared here.

    var account = builder.AddProject<Projects.CleanDDDArchitecture_Hosts_Services_AccountService_Presentation>("account-service")
        .WithHttpEndpoint()
        .WithTokenIssuance()
        .WithReference(postgres.AddDatabase("account-db", "cleanddd_account"), "PGSQLConnection")
        .WithReference(kafka, "kafka")
        .WithEnvironment("IntegrationEvents__Transport", "Kafka")
        .WithEnvironment("ConnectionStrings__kurrentdb", kurrentDbConnection)
        .WithEnvironment("EmailSettings__SmtpHost", smtp.Property(EndpointProperty.Host))
        .WithEnvironment("EmailSettings__SmtpPort", smtp.Property(EndpointProperty.Port))
        .WaitFor(postgres)
        .WaitFor(kafka)
        .WaitFor(kurrentDb);

    var todo = builder.AddProject<Projects.CleanDDDArchitecture_Hosts_Services_TodoService_Presentation>("todo-service")
        .WithHttpEndpoint()
        .WithTokenValidation()
        .WithReference(postgres.AddDatabase("todo-db", "cleanddd_todo"), "PGSQLConnection")
        .WithReference(kafka, "kafka")
        .WithEnvironment("IntegrationEvents__Transport", "Kafka")
        .WaitFor(postgres)
        .WaitFor(kafka);

    var weather = builder.AddProject<Projects.CleanDDDArchitecture_Hosts_Services_WeatherService_Presentation>("weather-service")
        .WithHttpEndpoint()
        .WithTokenValidation()
        .WithReference(postgres.AddDatabase("weather-db", "cleanddd_weather"), "PGSQLConnection")
        .WaitFor(postgres);

    builder.AddProject<Projects.CleanDDDArchitecture_Hosts_Gateway_Presentation>("gateway")
        .WithHttpEndpoint()
        .WithEnvironment("ReverseProxy__Clusters__account__Destinations__primary__Address", account.GetEndpoint("http"))
        .WithEnvironment("ReverseProxy__Clusters__todo__Destinations__primary__Address", todo.GetEndpoint("http"))
        .WithEnvironment("ReverseProxy__Clusters__weather__Destinations__primary__Address", weather.GetEndpoint("http"))
        .WithExternalHttpEndpoints()
        .WaitFor(account)
        .WaitFor(todo)
        .WaitFor(weather);
}
else
{
    // ── Monolith: every domain in one API, plus the background job worker ──

    var database = postgres.AddDatabase("monolith-db", "cleanddd");

    var api = builder.AddProject<Projects.CleanDDDArchitecture_Hosts_RestApi_Presentation>("restapi")
        .WithTokenIssuance()
        .WithReference(database, "PGSQLConnection")
        .WithReference(kafka, "kafka")
        .WithEnvironment("ConnectionStrings__kurrentdb", kurrentDbConnection)
        .WithEnvironment("EmailSettings__SmtpHost", smtp.Property(EndpointProperty.Host))
        .WithEnvironment("EmailSettings__SmtpPort", smtp.Property(EndpointProperty.Port))
        .WithExternalHttpEndpoints()
        .WaitFor(postgres)
        .WaitFor(kafka)
        .WaitFor(kurrentDb);

    // Hangfire: the API enqueues jobs, the worker runs them, both against the same database.
    builder.AddProject<Projects.CleanDDDArchitecture_Hosts_Worker>("worker")
        .WithReference(database, "PGSQLConnection")
        .WaitFor(api);
}

await builder.Build().RunAsync().ConfigureAwait(false);
