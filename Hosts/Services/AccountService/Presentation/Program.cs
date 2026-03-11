using AutoMapper;
using Aviant.Application.ApplicationEvents;
using Aviant.Application.Behaviours;
using Aviant.Application.Extensions;
using Aviant.Application.Interceptors;
using Aviant.Application.Jobs;
using Aviant.Application.Processors;
using Aviant.Application.Services;
using Aviant.Core.Timing;
using Aviant.Core.Messages;
using Aviant.Core.Services;
using Aviant.Infrastructure.CrossCutting;
using Aviant.Infrastructure.Email;
using Aviant.Infrastructure.Jobs;
using CleanDDDArchitecture.Domains.Account.CrossCutting;
using CleanDDDArchitecture.Domains.Shared.Core;
using CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Routing;
using CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Errors;
using CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Services;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.FeatureManagement;
using System.Globalization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddYamlFile("appsettings.yaml", false, true)
    .AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yaml", true, true)
    .AddEnvironmentVariables();

DependencyInjectionRegistry.ConfigurationBuilder = builder.Configuration;
DependencyInjectionRegistry.CurrentEnvironment = builder.Environment;
DependencyInjectionRegistry.SetConfiguration(builder.Configuration);
Clock.Provider = ClockProviders.Utc;

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();
builder.Services.AddSingleton<Aviant.Application.Identity.ICurrentUserService, CurrentUser>();
builder.Services.AddScoped<IMessages, Messages>();
builder.Services.AddScoped<IApplicationEventDispatcher, ApplicationEventDispatcher>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("public-auth", context => BuildIpPolicy(context, 5, TimeSpan.FromMinutes(1)));
    options.AddPolicy("public-recovery", context => BuildIpPolicy(context, 3, TimeSpan.FromMinutes(5)));
    options.AddPolicy("public-registration", context => BuildIpPolicy(context, 2, TimeSpan.FromMinutes(10)));
    options.AddPolicy("authenticated-sensitive", context => BuildIpPolicy(context, 10, TimeSpan.FromMinutes(1)));
});
var dataProtectionKeysPath = Environment.GetEnvironmentVariable("DataProtection__KeysPath")
    ?? Environment.GetEnvironmentVariable("DATA_PROTECTION_KEYS_PATH")
    ?? builder.Configuration["DataProtection:KeysPath"]
    ?? "DataProtection-Keys";
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.GetFullPath(dataProtectionKeysPath)));
builder.Services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>(_ => new SmtpClientFactory(
    builder.Configuration["EmailSettings:SmtpHost"],
    int.Parse(builder.Configuration["EmailSettings:SmtpPort"] ?? "1025", CultureInfo.InvariantCulture),
    bool.Parse(builder.Configuration["EmailSettings:EnableSsl"] ?? "false"),
    builder.Configuration["EmailSettings:SmtpUsername"],
    builder.Configuration["EmailSettings:SmtpPassword"]));
builder.Services.AddTransient<Aviant.Application.Email.IEmailService, EmailService>(provider =>
{
    var smtpClientFactory = provider.GetRequiredService<ISmtpClientFactory>();
    return new EmailService(smtpClientFactory, builder.Configuration["AppSettings:Title"], builder.Configuration["AppSettings:Emails:NoReply"]);
});

builder.Services.AddAutoMapper(cfg =>
{
    foreach (var profile in AccountCrossCutting.AutoMapperProfiles())
    {
        cfg.AddProfile(profile);
    }
});
builder.Services.AddValidatorsFromAssemblies(AccountCrossCutting.ValidatorAssemblies().ToArray());

builder.Services.AddTransient<IMediator, MediatR.Mediator>();
builder.Services.Scan(scan => scan.FromAssemblies(
        typeof(Program).Assembly,
        typeof(LoggerBehaviour<>).Assembly,
        AccountCrossCutting.MediatorAssemblies().Single())
    .RegisterHandlers(typeof(IRequestHandler<>))
    .RegisterHandlers(typeof(IRequestHandler<,>))
    .RegisterHandlers(typeof(InterceptorBase<>))
    .RegisterHandlers(typeof(INotificationHandler<>))
    .RegisterHandlers(typeof(IRequestPreProcessor<>))
    .RegisterHandlers(typeof(IRequestPostProcessor<,>))
    .RegisterHandlers(typeof(IRequestExceptionHandler<,,>))
    .RegisterHandlers(typeof(IRequestExceptionAction<,>)));
builder.Services.Decorate(typeof(IRequestHandler<,>), typeof(RetryRequestProcessor<,>));
builder.Services.Decorate(typeof(INotificationHandler<>), typeof(RetryEventProcessor<>));
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

builder.Services.AddAccountDomain();
builder.Services.AddFeatureManagement(DependencyInjectionRegistry.ConfigurationWithDomains);
builder.Services.AddHealthChecks().AddAccountChecks();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
        options.Conventions.Add(new CustomRouteConvention());
    })
    .AddApplicationPart(typeof(CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.ApiController).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await AccountCrossCutting.GenerateDefaultUserIfNotExistsAsync(scope.ServiceProvider).ConfigureAwait(false);
}

ServiceLocator.Initialise(app.Services);

app.UseApiExceptionHandling();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHealthChecks("/health");
app.UseSession();
app.UseRateLimiter();
app.UseRouting();
app.UseAccountAuth();
app.MapControllers().RequireAuthorization();
app.MapHangfireDashboard("/jobs");

await app.RunAsync().ConfigureAwait(false);

static RateLimitPartition<string> BuildIpPolicy(HttpContext context, int permitLimit, TimeSpan window)
{
    var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    return RateLimitPartition.GetFixedWindowLimiter(
        partitionKey,
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = window,
            QueueLimit = 0,
            AutoReplenishment = true
        });
}
