namespace CleanDDDArchitecture.RestApi
{
    using System;
    using System.Text;
    using Application;
    using Application.Accounts;
    using Application.Accounts.Commands.CreateAccount;
    using Aviant.DDD.Application;
    using Aviant.DDD.Application.EventBus;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Processors;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Domain.Aggregates;
    using Aviant.DDD.Domain.EventBus;
    using Aviant.DDD.Domain.Services;
    using Aviant.DDD.Infrastructure.Persistence.EventStore;
    using Aviant.DDD.Infrastructure.Persistence.Kafka;
    using Domain.Entities;
    using Filters;
    using Infrastructure;
    using Infrastructure.Persistence.Contexts;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Services;
    using Utils.Swagger;
    using Workers;

    /// <summary>
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.Configure<SwaggerSettings>(Configuration.GetSection(nameof(SwaggerSettings)));

            services.AddScoped<IMediator, Mediator>();
            // services.AddMediatR(typeof(CreateAccount).Assembly);
            services.Scan(
            scan =>
            {
                scan.FromAssembliesOf(typeof(CreateAccount), typeof(AccountCreatedEvent))
                    .RegisterHandlers(typeof(IRequestHandler<>))
                    .RegisterHandlers(typeof(IRequestHandler<,>))
                    .RegisterHandlers(typeof(INotificationHandler<>));
            });
            
            services
                .AddApplication()
                .AddInfrastructure(Configuration);

            services
                .AddSingleton<IEventDeserializer>(
                    new JsonEventDeserializer(
                        new[]
                        {
                            typeof(AccountCreatedEvent).Assembly,
                            typeof(CreateAccount).Assembly
                        }));
            
            var kafkaConnStr = Configuration.GetConnectionString("kafka");
            var eventsTopicName = Configuration["eventsTopicName"];
            var groupName = Configuration["eventsTopicGroupName"];
            var consumerConfig = new EventConsumerConfig(kafkaConnStr, eventsTopicName, groupName);
            
            var eventstoreConnStr = Configuration.GetConnectionString("eventstore");
            
            services.AddSingleton(consumerConfig)
                .AddSingleton(typeof(IEventConsumer<,,>), typeof(EventConsumer<,,>))
                .AddKafkaEventProducer<AccountEntity, AccountId>(consumerConfig);
            
            
            services.AddSingleton<IEventStoreConnectionWrapper>(
                    ctx =>
                    {
                        var logger = ctx.GetRequiredService<ILogger<EventStoreConnectionWrapper>>();
                        return new EventStoreConnectionWrapper(new Uri(eventstoreConnStr), logger);
                    })
                .AddEventsRepository<AccountEntity, AccountId>();
            
            
            services.AddEventsService<AccountEntity, AccountId>();

            services.AddScoped<ServiceFactory>(ctx => ctx.GetRequiredService);

            services.Decorate(typeof(INotificationHandler<>), typeof(RetryProcessor<>));
            
            services.AddSingleton<IEventConsumerFactory, EventConsumerFactory>();

            services.AddHostedService(
                ctx =>
                {
                    var factory = ctx.GetRequiredService<IEventConsumerFactory>();
                    return new EventsConsumerWorker(factory);
                });
            
            services
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["Jwt:Issuer"],
                            ValidAudience = Configuration["Jwt:Issuer"],
                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            services
                .AddApiVersionWithExplorer()
                .AddSwaggerOptions()
                .AddSwaggerGen();

            services.AddSingleton<ICurrentUserService, CurrentUser>();

            services.AddHttpContextAccessor();

            services
                .AddHealthChecks()
                .AddDbContextCheck<TodoDbContext>();

            // services.AddControllersWithViews(
            //     options =>
            //     {
            //         options.Filters.Add(new ApiExceptionFilter());
            //         //options.Filters.Add(new AuthorizeFilter());
            //     }
            // );
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
            IApiVersionDescriptionProvider provider,
            IServiceProvider serviceProvider)
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
                    DefaultContentType = "application/yaml"
                });


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints
                        .MapDefaultControllerRoute();
                    // .RequireAuthorization();
                });
        }
    }
}