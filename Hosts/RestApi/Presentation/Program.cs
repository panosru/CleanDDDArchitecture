using CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Presentation.Endpoints;
using CleanDDDArchitecture.Hosts.ServiceDefaults.Core;
using CleanDDDArchitecture.Hosts.RestApi.Presentation;
using CleanDDDArchitecture.Hosts.RestApi.Presentation.Setup;
using CleanDDDArchitecture.Hosts.RestApi.Core.Resources;
using Serilog;
using Serilog.Debugging;
// Bootstrap logger: startup failures are logged even before configuration is read.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    // Create a Web Presentation Builder. This is the first step in setting up an ASP.NET Core application.
    var builder = WebApplication.CreateBuilder(args);

    // Load the YAML configuration first: the Serilog section lives there.
    new ConfigurationSetup().Setup(builder);

    // Replace the bootstrap logger with the configured one. Creating it before the YAML
    // files were loaded produced a logger with no sinks, which swallowed every startup error.
    Log.Logger = new LoggerConfiguration()
         .ReadFrom.Configuration(builder.Configuration)
         .CreateLogger();

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    builder.AddServiceDefaults();

    // Setup services using the ServicesSetup class
    new ServicesSetup().Setup(builder.Services);

    // Configure services using extension method from ServiceConfiguration class
    builder.Services.ConfigureServices(builder.Configuration, builder.Environment);
    
    // Build the application after all services have been registered
    var app = builder.Build();
    
    // Migrate and seed database using DatabaseSetup class
    await new DatabaseSetup().MigrateAndSeedDatabase(app.Services)
        .ConfigureAwait(false);

    // Enable self logging for Serilog
    SelfLog.Enable(Console.Error);
    
    // Configure middleware pipeline using extension method from AppBuilderConfiguration class
    app.ConfigureAppBuilder(app.Services, builder.Environment);
    app.MapDefaultEndpoints();
    app.MapWeatherEndpoints();
    
    // Run the application
    await app.RunAsync()
        .ConfigureAwait(false);
}
catch (Exception e)
    // Ignore HostAbortedException that is thrown when the application is stopped using Ctrl+C
    when (e is not HostAbortedException)
{
    Log.Fatal(e, Resource.HostTerminatedUnexpectedly);

    // A failed start must not look like a clean exit to whatever supervises the process.
    Environment.ExitCode = 1;
}
finally
{
    // Ensure that all log events have been flushed to their sinks before the program exits
    Log.CloseAndFlush();
}
