var builder = DistributedApplication.CreateBuilder(args);

// ── Infrastructure ───────────────────────────────────────────────────────────

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "cleandddarchitecture")
    .WithDataVolume("postgres-data");

var accountDb  = postgres.AddDatabase("account-db",  "cleandddarchitecture_account");
var todoDb     = postgres.AddDatabase("todo-db",     "cleandddarchitecture_todo");
var weatherDb  = postgres.AddDatabase("weather-db",  "cleandddarchitecture_weather");
var monolithDb = postgres.AddDatabase("monolith-db", "cleandddarchitecture");

var redis = builder.AddRedis("redis")
    .WithDataVolume("redis-data");

var mailpit = builder.AddContainer("mailpit", "axllent/mailpit")
    .WithHttpEndpoint(port: 8025, targetPort: 8025, name: "ui")
    .WithEndpoint(port: 1025, targetPort: 1025, name: "smtp");

// ── Microservices mode ───────────────────────────────────────────────────────

builder.AddProject<Projects.CleanDDDArchitecture_Hosts_Services_AccountService_Presentation>("account-service")
    .WithReference(accountDb)
    .WithReference(redis)
    .WithReference(mailpit.GetEndpoint("smtp"))
    .WaitFor(postgres)
    .WaitFor(redis);

builder.AddProject<Projects.CleanDDDArchitecture_Hosts_Services_TodoService_Presentation>("todo-service")
    .WithReference(todoDb)
    .WithReference(redis)
    .WaitFor(postgres)
    .WaitFor(redis);

builder.AddProject<Projects.CleanDDDArchitecture_Hosts_Services_WeatherService_Presentation>("weather-service")
    .WithReference(weatherDb)
    .WithReference(redis)
    .WaitFor(postgres)
    .WaitFor(redis);

// ── Monolith mode ────────────────────────────────────────────────────────────

builder.AddProject<Projects.CleanDDDArchitecture_Hosts_RestApi_Presentation>("restapi")
    .WithReference(monolithDb)
    .WithReference(redis)
    .WithReference(mailpit.GetEndpoint("smtp"))
    .WaitFor(postgres)
    .WaitFor(redis);

await builder.Build().RunAsync().ConfigureAwait(false);
