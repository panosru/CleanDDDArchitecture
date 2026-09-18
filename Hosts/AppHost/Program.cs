// Local orchestration with .NET Aspire.
//
//   dotnet run --project Hosts/AppHost                          # monolith (default)
//   dotnet run --project Hosts/AppHost -- --mode microservices  # one service per domain + gateway
//
// Both modes run the same domain code; only the hosts differ. Integration events between
// contexts travel in-process in the monolith and over Kafka between the services. Every service receives its
// settings under the names it already reads (ConnectionStrings:PGSQLConnection, :kafka,
// :eventstore, EmailSettings:*), so nothing in the services knows about Aspire beyond
// AddServiceDefaults(), which sends traces, metrics and logs to the Aspire dashboard.

var builder = DistributedApplication.CreateBuilder(args);

var microservices = string.Equals(builder.Configuration["mode"], "microservices", StringComparison.OrdinalIgnoreCase);

// ── Infrastructure ───────────────────────────────────────────────────────────

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("cleanddd-postgres-data");

var kafka = builder.AddKafka("kafka");

// EventStoreDB 21.10 over TCP, the protocol the current event store client speaks.
var eventStore = builder.AddContainer("eventstore", "eventstore/eventstore", "21.10.11-buster-slim")
    .WithEnvironment("EVENTSTORE_CLUSTER_SIZE", "1")
    .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
    .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "true")
    .WithEnvironment("EVENTSTORE_INSECURE", "true")
    .WithEnvironment("EVENTSTORE_ENABLE_EXTERNAL_TCP", "true")
    .WithEnvironment("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "true")
    .WithEndpoint(targetPort: 1113, name: "tcp", scheme: "tcp")
    .WithHttpEndpoint(targetPort: 2113, name: "http");

var eventStoreConnection = ReferenceExpression.Create(
    $"tcp://admin:changeit@{eventStore.GetEndpoint("tcp").Property(EndpointProperty.HostAndPort)}");

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
        .WithEnvironment("ConnectionStrings__eventstore", eventStoreConnection)
        .WithEnvironment("EmailSettings__SmtpHost", smtp.Property(EndpointProperty.Host))
        .WithEnvironment("EmailSettings__SmtpPort", smtp.Property(EndpointProperty.Port))
        .WaitFor(postgres)
        .WaitFor(kafka)
        .WaitFor(eventStore);

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
        .WithEnvironment("ConnectionStrings__eventstore", eventStoreConnection)
        .WithEnvironment("EmailSettings__SmtpHost", smtp.Property(EndpointProperty.Host))
        .WithEnvironment("EmailSettings__SmtpPort", smtp.Property(EndpointProperty.Port))
        .WithExternalHttpEndpoints()
        .WaitFor(postgres)
        .WaitFor(kafka)
        .WaitFor(eventStore);

    // Hangfire: the API enqueues jobs, the worker runs them, both against the same database.
    builder.AddProject<Projects.CleanDDDArchitecture_Hosts_Worker>("worker")
        .WithReference(database, "PGSQLConnection")
        .WaitFor(api);
}

await builder.Build().RunAsync().ConfigureAwait(false);
