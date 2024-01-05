using CleanDDDArchitecture.Hosts.RestApi.Presentation;
using CleanDDDArchitecture.Hosts.RestApi.Presentation.Setup;
using CleanDDDArchitecture.Hosts.RestApi.Core.Resources;
using Serilog;
using Serilog.Debugging;
using Console = Colorful.Console;

try
{
    // Create a Web Presentation Builder. This is the first step in setting up an ASP.NET Core application.
    var builder = WebApplication.CreateBuilder(args);

    // Create a new logger for the application using Serilog
    Log.Logger = new LoggerConfiguration()
         .ReadFrom.Configuration(builder.Configuration)
         .CreateLogger();
    
    // Use Serilog as the logger for the application
    builder.Host.UseSerilog((context, configuration) => 
        configuration.ReadFrom.Configuration(context.Configuration));

    // Setup configuration using the ConfigurationSetup class
    new ConfigurationSetup().Setup(builder);

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
    
    // Run the application
    await app.RunAsync()
        .ConfigureAwait(false);
}
catch (Exception e)
    // Ignore HostAbortedException that is thrown when the application is stopped using Ctrl+C
    when (e is not HostAbortedException)
{
    // Log any fatal exception that occurs and print it on the console
    Log.Fatal(e, Resource.HostTerminatedUnexpectedly);
    Console.WriteLine(e);
}
finally
{
    // Ensure that all log events have been flushed to their sinks before the program exits
    Log.CloseAndFlush();
}
