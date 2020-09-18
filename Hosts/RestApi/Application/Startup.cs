namespace CleanDDDArchitecture.Hosts.RestApi.Application
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;
    using AutoMapper;
    using Aviant.DDD.Application.Behaviours;
    using Aviant.DDD.Application.EventBus;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Notifications;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Core.Messages;
    using Aviant.DDD.Core.Services;
    using Aviant.DDD.Infrastructure.CrossCutting;
    using Aviant.DDD.Infrastructure.Services;
    using Domains.Account.CrossCutting;
    using Domains.Todo.CrossCutting;
    using Domains.Weather.CrossCutting;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Services;
    using Utils.Swagger;
    using IServiceContainer = Aviant.DDD.Core.Services.IServiceContainer;

    // using Workers;

    /// <summary>
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="currentEnvironment"></param>
        public Startup(
            IConfiguration      configuration,
            IWebHostEnvironment currentEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment   = currentEnvironment;
        }

        private IConfiguration Configuration { get; }
        
        private IWebHostEnvironment CurrentEnvironment { get; }

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


            services.AddTransient<IMediator, Mediator>();

            services.Scan(
                scan =>
                {
                    scan.FromAssemblies(
                            TodoCrossCutting.MediatorAssemblies()
                               .Union(AccountCrossCutting.MediatorAssemblies())
                               .Union(WeatherCrossCutting.MediatorAssemblies())
                               .ToArray())
                       .RegisterHandlers(typeof(IRequestHandler<>))
                       .RegisterHandlers(typeof(IRequestHandler<,>))
                       .RegisterHandlers(typeof(MediatR.INotificationHandler<>));
                });


            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(PerformanceBehaviour<,>));

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehaviour<,>));

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(UnhandledExceptionBehaviour<,>));


            // Add Infrastructure
            services
               .AddAccountDomain()
               .AddTodoDomain()
               .AddWeatherDomain();


            services.AddTransient<IDateTimeService, DateTimeService>();

            services.AddScoped<IMessages, Messages>();
            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

            services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();

            services.AddAccountAuth();

            services
               .AddApiVersionWithExplorer(Configuration)
               .AddSwaggerOptions()
               .AddSwaggerGen();

            services.AddSingleton<ICurrentUserService, CurrentUser>();

            services.AddHttpContextAccessor();

            services
               .AddHealthChecks()
               .AddAccountChecks()
               .AddTodoChecks();

            services.AddControllersWithViews(
                options =>
                {
                    // options.Filters.Add(new ApiExceptionFilter());
                    //options.Filters.Add(new AuthorizeFilter());
                }
            );
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
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios,
                // see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHealthChecks("/health");

            app.UseSwaggerDocuments();

            app.UseHttpsRedirection();

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    ServeUnknownFileTypes = true,
                    DefaultContentType    = "application/yaml"
                });


            app.UseRouting();

            app.UseAccountAuth();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints
                       .MapDefaultControllerRoute()
                       .RequireAuthorization();
                });
        }
    }
}