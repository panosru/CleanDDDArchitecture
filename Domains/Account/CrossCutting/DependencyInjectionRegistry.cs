namespace CleanDDDArchitecture.Domains.Account.CrossCutting
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Application.Aggregates;
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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class DependencyInjectionRegistry
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
            
            services.AddDefaultIdentity<AccountUser>()
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
                    .AddKafkaEventProducer<AccountEntity, AccountId>(consumerConfig);
            
            
            services.AddSingleton<IEventStoreConnectionWrapper>(
                         ctx =>
                         {
                             var logger = ctx.GetRequiredService<ILogger<EventStoreConnectionWrapper>>();
            
                             return new EventStoreConnectionWrapper(new Uri(eventstoreConnStr), logger);
                         })
                    .AddEventsRepository<AccountEntity, AccountId>();
            
            
            services.AddEventsService<AccountEntity, AccountId>();
            
            services.Decorate(typeof(INotificationHandler<>), typeof(RetryProcessor<>));

            services.AddSingleton<IEventConsumerFactory, EventConsumerFactory>();

            services.AddHostedService(
                ctx =>
                {
                    var factory = ctx.GetRequiredService<IEventConsumerFactory>();
            
                    return new EventsConsumerWorker(factory);
                });

            services.AddScoped<IOrchestrator<AccountEntity, AccountId>, Orchestrator<AccountEntity, AccountId>>();
            services.AddScoped<IUnitOfWork<AccountEntity, AccountId>, UnitOfWork<AccountEntity, AccountId>>();

            return services;
        }
    }
}