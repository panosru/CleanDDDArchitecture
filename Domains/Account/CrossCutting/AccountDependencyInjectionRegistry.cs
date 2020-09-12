namespace CleanDDDArchitecture.Domains.Account.CrossCutting
{
    #region

    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;
    using Application.Aggregates;
    using Application.Identity;
    using Application.Persistence;
    using Application.Repositories;
    using Application.UseCases.Create;
    using Application.UseCases.Create.Events;
    using Aviant.DDD.Application.EventBus;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.Persistance;
    using Aviant.DDD.Application.Processors;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Domain.EventBus;
    using Aviant.DDD.Domain.Services;
    using Aviant.DDD.Infrastructure.Persistence;
    using Aviant.DDD.Infrastructure.Persistence.EventStore;
    using Aviant.DDD.Infrastructure.Persistence.Kafka;
    using Infrastructure.Identity;
    using Infrastructure.Persistence.Contexts;
    using Infrastructure.Repositories;
    using Infrastructure.Workers;
    using MediatR;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    #endregion

    public static class AccountDependencyInjectionRegistry
    {
        public static IServiceCollection AddAccount(
            this IServiceCollection services,
            IConfiguration          configuration)
        {
            // By default, Microsoft has some legacy claim mapping that converts
            // standard JWT claims into proprietary ones. This removes those mappings.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            services.AddDbContext<AccountDbContextWrite>(
                options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultWriteConnection"),
                        b =>
                            b.MigrationsAssembly(typeof(AccountDbContextWrite).Assembly.FullName)));

            services.AddScoped<IAccountDbContextWrite>(
                provider =>
                    provider.GetService<AccountDbContextWrite>());

            services.AddDbContext<AccountDbContextRead>(
                options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultReadConnection"),
                        b =>
                            b.MigrationsAssembly(typeof(IAccountDbContextRead).Assembly.FullName)));

            services.AddScoped<IAccountDbContextRead>(
                provider =>
                    provider.GetService<AccountDbContextRead>());

            services.AddScoped<IAccountRepositoryRead, AccountRepositoryRead>();
            services.AddScoped<IAccountRepositoryWrite, AccountRepositoryWrite>();

            services
               .AddDefaultIdentity<AccountUser>(
                    options => { options.User.RequireUniqueEmail = true; })
               .AddRoles<AccountRole>()
               .AddEntityFrameworkStores<AccountDbContextWrite>();

            services.AddIdentityServer()
               .AddApiAuthorization<AccountUser, AccountDbContextWrite>();

            services.AddTransient<IIdentityService, IdentityService>();

            services.AddAuthentication()
               .AddIdentityServerJwt();

            services
               .AddSingleton<IEventDeserializer>(
                    new JsonEventDeserializer(
                        new[]
                        {
                            typeof(AccountCreatedEvent).Assembly,
                            typeof(CreateAccount).Assembly
                        }));

            var kafkaConnStr    = configuration.GetConnectionString("kafka");
            var eventsTopicName = configuration["eventsTopicName"];
            var groupName       = configuration["eventsTopicGroupName"];
            var consumerConfig  = new EventConsumerConfig(kafkaConnStr, eventsTopicName, groupName);

            var eventstoreConnStr = configuration.GetConnectionString("eventstore");

            services.AddSingleton(consumerConfig)
               .AddSingleton(typeof(IEventConsumer<,,>), typeof(EventConsumer<,,>))
               .AddKafkaEventProducer<AccountAggregate, AccountAggregateId>(consumerConfig);


            services.AddSingleton<IEventStoreConnectionWrapper>(
                    ctx =>
                    {
                        var logger = ctx.GetRequiredService<ILogger<EventStoreConnectionWrapper>>();

                        return new EventStoreConnectionWrapper(new Uri(eventstoreConnStr), logger);
                    })
               .AddEventsRepository<AccountAggregate, AccountAggregateId>();


            services.AddEventsService<AccountAggregate, AccountAggregateId>();

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
               .AddScoped<IOrchestrator<AccountAggregate, AccountAggregateId>,
                    Orchestrator<AccountAggregate, AccountAggregateId>>();

            services
               .AddScoped<IUnitOfWork<AccountAggregate, AccountAggregateId>,
                    UnitOfWork<AccountAggregate, AccountAggregateId>>();

            return services;
        }

        public static IServiceCollection AddAccountAuth(
            this IServiceCollection services,
            IConfiguration          configuration)
        {
            services
               .AddAuthorization()
               .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(
                    options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer           = true,
                            ValidateAudience         = true,
                            ValidateLifetime         = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer              = configuration["Jwt:Issuer"],
                            ValidAudience            = configuration["Jwt:Issuer"],
                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            return services;
        }

        public static IHealthChecksBuilder AddAccountChecks(this IHealthChecksBuilder builder) =>
            builder.AddDbContextCheck<AccountDbContextWrite>();

        public static void UseAccountAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}