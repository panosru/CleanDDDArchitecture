namespace CleanDDDArchitecture.Hosts.RestApi.Application;

using System.Reflection;
using AutoMapper;
using Aviant.Application.ApplicationEvents;
using Aviant.Application.Behaviours;
using Aviant.Application.EventSourcing.EventBus;
using Aviant.Application.Extensions;
using Aviant.Application.Identity;
using Aviant.Application.Interceptors;
using Aviant.Application.Jobs;
using Aviant.Application.Processors;
using Aviant.Application.Services;
using Aviant.Core.Messages;
using Aviant.Core.Services;
using Aviant.Infrastructure.CrossCutting;
using Aviant.Infrastructure.Jobs;
using Domains.Account.CrossCutting;
using Domains.Shared.Core;
using Domains.Todo.CrossCutting;
using Domains.Weather.CrossCutting;
using Features;
using Filters;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Middlewares;
using Services;
using Swagger;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Serilog;
using Serilog.Events;

/// <summary>
/// </summary>
public sealed class Startup
{
    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="currentEnvironment"></param>
    public Startup(
        IConfiguration      configuration,
        IWebHostEnvironment currentEnvironment)
    {
        Configuration      = configuration;
        CurrentEnvironment = currentEnvironment;
    }

    /// <summary>
    /// </summary>
    private IConfiguration Configuration { get; }

    /// <summary>
    /// </summary>
    private IWebHostEnvironment CurrentEnvironment { get; }

    /// <summary>
    /// Data protection Keys Directory
    /// </summary>
    private readonly string _dataProtectionKeysDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DataProtection-Keys");

    /// <summary>
    ///     This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);
        DependencyInjectionRegistry.CurrentEnvironment = CurrentEnvironment;

        services.AddAutoMapper(
            cfg =>
            {
                IEnumerable<Profile> profiles = TodoCrossCutting.AutoMapperProfiles()
                   .Union(AccountCrossCutting.AutoMapperProfiles())
                   .Union(WeatherCrossCutting.AutoMapperProfiles())
                   .ToList();

                foreach (var profile in profiles)
                    cfg.AddProfile(profile);
            });

        services.AddValidatorsFromAssemblies(
            TodoCrossCutting.ValidatorAssemblies()
               .Union(AccountCrossCutting.ValidatorAssemblies())
               .Union(WeatherCrossCutting.ValidatorAssemblies())
               .ToArray());

        services.AddDataProtection()
           .PersistKeysToFileSystem(new DirectoryInfo(_dataProtectionKeysDirectory))
           .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

        services.AddTransient<IMediator, Mediator>();

        services.Scan(
            scan => scan.FromAssemblies(
                    new List<Assembly>
                        {
                            typeof(Startup).Assembly,
                            typeof(LoggerBehaviour<>).Assembly
                        }
                       .Union(TodoCrossCutting.MediatorAssemblies())
                       .Union(AccountCrossCutting.MediatorAssemblies())
                       .Union(WeatherCrossCutting.MediatorAssemblies())
                       .ToArray())
               .RegisterHandlers(typeof(IRequestHandler<>))
               .RegisterHandlers(typeof(IRequestHandler<,>))
               .RegisterHandlers(typeof(InterceptorBase<>))
               .RegisterHandlers(typeof(INotificationHandler<>))
               .RegisterHandlers(typeof(IRequestPreProcessor<>))
               .RegisterHandlers(typeof(IRequestPostProcessor<,>))
               .RegisterHandlers(typeof(IRequestExceptionHandler<,,>))
               .RegisterHandlers(typeof(IRequestExceptionAction<,>)));

        services.Decorate(
            typeof(IRequestHandler<,>),
            typeof(RetryRequestProcessor<,>));

        services.Decorate(
            typeof(INotificationHandler<>),
            typeof(RetryEventProcessor<>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(PerformanceBehaviour<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(UnhandledExceptionBehaviour<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(RequestPreProcessorBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(RequestPostProcessorBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(RequestExceptionActionProcessorBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(RequestExceptionProcessorBehavior<,>));

        services
           .AddDistributedMemoryCache()
           .AddSession(
                options =>
                {
                    options.IdleTimeout        = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly    = true;
                    options.Cookie.IsEssential = true;
                });


        services.AddSingleton<IJobRunner, JobRunner>();

        services.AddHangfire(
                config => config
                   .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UsePostgreSqlStorage(
                        Configuration.GetConnectionString("Hangfire"),
                        new PostgreSqlStorageOptions
                        {
                            QueuePollInterval = TimeSpan.FromSeconds(15)
                        })
                   .UseFilter(
                        new AutomaticRetryAttribute
                        {
                            Attempts = 5
                        }))
           .AddHangfireServer(
                options =>
                {
                    options.ServerName  = $"{Environment.MachineName}.{Guid.NewGuid().ToString()}";
                    options.WorkerCount = Environment.ProcessorCount * 5;
                    options.Queues      = new[] { JobQueue.Main, JobQueue.Second, JobQueue.Default };
                });

        // Add Infrastructure
        services
           .AddAccountDomain()
           .AddTodoDomain()
           .AddWeatherDomain();

        services.AddFeatureFlags();

        services.AddScoped<IMessages, Messages>();
        services.AddScoped<IApplicationEventDispatcher, ApplicationEventDispatcher>();

        services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();

        services
           .AddApiVersionWithExplorer()
           .AddSwaggerOptions()
           .AddSwaggerGen();

        services.AddSingleton<ICurrentUserService, CurrentUser>();

        services.AddHttpContextAccessor();

        services
           .AddHealthChecks()
           .AddAccountChecks()
           .AddTodoChecks();

        services.AddRouting(
            options => options.LowercaseUrls = true);

        if (CurrentEnvironment.IsDevelopment())
            services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddControllersWithViews(
            options =>
            {
                options.Filters.Add(new ApiExceptionFilterAttribute());
                options.Filters.Add(new AuthorizeFilter());
            });
    }

    /// <summary>
    ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="serviceProvider"></param>
    public void Configure(
        IApplicationBuilder app,
        IServiceProvider    serviceProvider)
    {
        ServiceLocator.Initialise(serviceProvider);

        if (CurrentEnvironment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios,
            // see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseCustomExceptionHandler();
        }

        app.UseSerilogRequestLogging(
            options =>
            {
                // Customize the message template
                options.MessageTemplate = "Handled {RequestPath}";

                // Emit debug-level events instead of the defaults
                options.GetLevel = (
                    _ /* httpContext */,
                    _ /* elapsed */,
                    _ /* Exception */) => LogEventLevel.Debug;

                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost",   httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                };
            });

        app.UseHangfireDashboard(
            "/jobs",
            new DashboardOptions
            {
                DashboardTitle = "Jobs",
                Authorization = new[]
                {
                    new HangfireAuthorizationFilter()
                },
                IgnoreAntiforgeryToken = true
            });

        app.UseHealthChecks("/health");

        app.UseSwaggerDocuments();

        app.UseHttpsRedirection();

        app.UseStaticFiles(
            new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType    = "application/yaml"
            });

        app.UseSession();

        app.UseRouting();

        app.UseAccountAuth();

        app.UseEndpoints(
            endpoints =>
            {
                endpoints
                   .MapDefaultControllerRoute()
                   .RequireAuthorization();

                endpoints.MapHangfireDashboard();
            });
    }
}
