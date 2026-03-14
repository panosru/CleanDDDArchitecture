using System.Globalization;
using System.Text;
using Aviant.Application.ApplicationEvents;
using Aviant.Application.Behaviours;
using Aviant.Application.Extensions;
using Aviant.Application.Identity;
using Aviant.Application.Interceptors;
using Aviant.Application.Jobs;
using Aviant.Application.Processors;
using Aviant.Application.Services;
using Aviant.Core.Timing;
using Aviant.Core.Messages;
using Aviant.Core.Services;
using Aviant.Infrastructure.CrossCutting;
using Aviant.Infrastructure.Jobs;
using CleanDDDArchitecture.Domains.Shared.Core;
using CleanDDDArchitecture.Domains.Weather.CrossCutting;
using CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Routing;
using CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Errors;
using CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Services;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Authorization;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddYamlFile("appsettings.yaml", false, true)
    .AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yaml", true, true)
    .AddEnvironmentVariables();

DependencyInjectionRegistry.ConfigurationBuilder = builder.Configuration;
DependencyInjectionRegistry.CurrentEnvironment = builder.Environment;
DependencyInjectionRegistry.SetConfiguration(builder.Configuration);
Clock.Provider = ClockProviders.Utc;
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUser>();
builder.Services.AddScoped<IMessages, Messages>();
builder.Services.AddScoped<IApplicationEventDispatcher, ApplicationEventDispatcher>();
var dataProtectionKeysPath = Environment.GetEnvironmentVariable("DataProtection__KeysPath")
    ?? Environment.GetEnvironmentVariable("DATA_PROTECTION_KEYS_PATH")
    ?? builder.Configuration["DataProtection:KeysPath"]
    ?? "DataProtection-Keys";
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.GetFullPath(dataProtectionKeysPath)));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Access:Key256Bit"])),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Access:Key512Bit"])),
            ClockSkew = TimeSpan.FromMinutes(double.Parse(builder.Configuration["Jwt:ClockSkewInMinutes"] ?? "0", CultureInfo.InvariantCulture))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(cfg =>
{
    foreach (var profile in WeatherCrossCutting.AutoMapperProfiles())
    {
        cfg.AddProfile(profile);
    }
});
builder.Services.AddValidatorsFromAssemblies(WeatherCrossCutting.ValidatorAssemblies().ToArray());

builder.Services.AddScoped<ServiceFactory>(ctx => ctx.GetRequiredService);
builder.Services.AddTransient<IMediator, MediatR.Mediator>();
builder.Services.Scan(scan => scan.FromAssemblies(
        typeof(Program).Assembly,
        typeof(LoggerBehaviour<>).Assembly,
        WeatherCrossCutting.MediatorAssemblies().Single())
    .RegisterHandlers(typeof(IRequestHandler<>))
    .RegisterHandlers(typeof(IRequestHandler<,>))
    .RegisterHandlers(typeof(InterceptorBase<>))
    .RegisterHandlers(typeof(INotificationHandler<>))
    .RegisterHandlers(typeof(IRequestPreProcessor<>))
    .RegisterHandlers(typeof(IRequestPostProcessor<,>))
    .RegisterHandlers(typeof(IRequestExceptionHandler<,,>))
    .RegisterHandlers(typeof(IRequestExceptionAction<,>)));
builder.Services.Decorate(typeof(IRequestHandler<,>), typeof(RetryRequestProcessor<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionActionProcessorBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));

builder.Services.AddSingleton<IJobRunner, JobRunner>();
builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("PGSQLConnection"))))
    .AddHangfireServer(options =>
    {
        options.ServerName = $"{Environment.MachineName}.{Guid.NewGuid()}";
        options.Queues = ["main", "second", "third", "default"];
    });

builder.Services.AddWeatherDomain();
builder.Services.AddFeatureManagement(DependencyInjectionRegistry.ConfigurationWithDomains);
builder.Services.AddHealthChecks();
builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
    })
    .AddMvc(options =>
    {
        options.Conventions.Add(new VersionByNamespaceConvention());
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v1.1");
builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
        options.Conventions.Add(new CustomRouteConvention());
    })
    .AddApplicationPart(typeof(CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Presentation.ApiController).Assembly);

var app = builder.Build();

ServiceLocator.Initialise(app.Services);

app.UseApiExceptionHandling();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHealthChecks("/health");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

await app.RunAsync().ConfigureAwait(false);
