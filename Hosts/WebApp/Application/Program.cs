using CleanDDDArchitecture.Hosts.WebApp.Application;

// Create a Web Application Builder. This is the first step in setting up an ASP.NET Core application.
var builder = WebApplication.CreateBuilder(args);

// Configure services using extension method from ServiceConfiguration class
builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

// Build the application after all services have been registered
var app = builder.Build();

// Configure middleware pipeline using extension method from AppBuilderConfiguration class
app.ConfigureAppBuilder(app.Services, builder.Environment);

app.Run();
