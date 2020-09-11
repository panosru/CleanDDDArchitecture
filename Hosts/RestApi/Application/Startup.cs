namespace CleanDDDArchitecture.Hosts.RestApi.Application
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Aviant.DDD.Application.Behaviours;
    using Aviant.DDD.Application.EventBus;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Notifications;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Domain.Messages;
    using Aviant.DDD.Domain.Services;
    using Aviant.DDD.Infrastructure.Services;
    using Domains.Account.CrossCutting;
    using Domains.Todo.CrossCutting;
    using Domains.Weather.CrossCutting;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Services;
    using Utils.Swagger;
    // using Domains.Weather.Application.UseCases.Forecast;
    // using Domains.Account.Application.Aggregates;
    // using Domains.Account.Application.UseCases.Create;
    // using Domains.Account.Application.UseCases.Create.Events;
    // using Domains.Todo.Infrastructure.Persistence.Contexts;

    #endregion

    // using Workers;

    /// <summary>
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.AddAutoMapper(
                cfg =>
                {
                    IEnumerable<Profile> profiles = TodoCrossCutting.TodoAutoMapperProfiles()
                       .Union(AccountCrossCutting.AccountAutoMapperProfiles())
                       .Union(WeatherCrossCutting.WeatherAutoMapperProfiles())
                       .ToList();

                    foreach (var profile in profiles)
                        cfg.AddProfile(profile);
                });

            services.AddValidatorsFromAssemblies(
                TodoCrossCutting.TodoValidatorAssemblies()
                   .Union(
                        AccountCrossCutting.AccountValidatorAssemblies())
                   .Union(
                        WeatherCrossCutting.WeatherValidatorAssemblies())
                   .ToArray());


            services.AddTransient<IMediator, Mediator>();

            services.Scan(
                scan =>
                {
                    scan.FromAssemblies(
                            TodoCrossCutting.TodoMediatorAssemblies()
                               .Union(
                                    AccountCrossCutting.AccountMediatorAssemblies())
                               .Union(
                                    WeatherCrossCutting.WeatherMediatorAssemblies())
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
               .AddAccount(Configuration)
               .AddTodo(Configuration)
               .AddWeather(Configuration);


            services.AddTransient<IDateTimeService, DateTimeService>();

            services.AddScoped<IMessages, Messages>();
            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

            services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();

            services.AddAccountAuth(Configuration);

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
        /// <param name="env"></param>
        /// <param name="provider"></param>
        /// <param name="serviceProvider"></param>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider    serviceProvider)
        {
            ServiceLocator.Initialise(serviceProvider.GetService<IServiceContainer>());

            if (env.IsDevelopment())
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