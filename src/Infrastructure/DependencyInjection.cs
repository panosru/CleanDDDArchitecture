namespace CleanDDDArchitecture.Infrastructure
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Application.Persistence;
    using Application.Repositories;
    using Application.TodoLists.Queries.ExportTodos;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Notifications;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Domain.Messages;
    using Aviant.DDD.Domain.Persistence;
    using Aviant.DDD.Domain.Services;
    using Aviant.DDD.Infrastructure;
    using Aviant.DDD.Infrastructure.Persistence;
    using Aviant.DDD.Infrastructure.Persistence.EventStore;
    using Aviant.DDD.Infrastructure.Persistence.Kafka;
    using Aviant.DDD.Infrastructure.Services;
    using Domain.Entities;
    using Files.Maps;
    using Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence.Contexts;
    using Repositories;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // By default, Microsoft has some legacy claim mapping that converts
            // standard JWT claims into proprietary ones. This removes those mappings.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<TodoDbContext>(
                    options =>
                        options.UseInMemoryDatabase("CleanDDDArchitectureDb"));

                services.AddDbContext<TodoDbContextReadOnly>(
                    options =>
                        options.UseInMemoryDatabase("CleanDDDArchitectureDb"));
            }
            else
            {
                services.AddDbContext<TodoDbContext>(
                    options =>
                        options.UseNpgsql(
                            configuration.GetConnectionString("DefaultWriteConnection"),
                            b =>
                                b.MigrationsAssembly(typeof(TodoDbContext).Assembly.FullName)));

                services.AddDbContext<TodoDbContextReadOnly>(
                    options =>
                        options.UseNpgsql(
                            configuration.GetConnectionString("DefaultReadConnection"),
                            b =>
                                b.MigrationsAssembly(typeof(TodoDbContextReadOnly).Assembly.FullName)));
            }

            services.AddScoped<ITodoDbContext>(
                provider =>
                    provider.GetService<TodoDbContext>());

            services.AddScoped<ITodoDbContextReadOnly>(
                provider =>
                    provider.GetService<TodoDbContextReadOnly>());

            #region Read Repositories

            services.AddScoped<ITodoItemReadRepository, TodoItemReadRepository>();
            services.AddScoped<ITodoListReadRepository, TodoListReadRepository>();
            // services.AddScoped<IAccountReadRepository, AccountReadRepository>();

            #endregion

            #region Write Repositories

            services.AddScoped<ITodoItemWriteRepository, TodoItemWriteRepository>();
            services.AddScoped<ITodoListWriteRepository, TodoListWriteRepository>();
            // services.AddScoped<IAccountWriteRepository, AccountWriteRepository>();

            #endregion

            services.AddDefaultIdentity<TodoUser>()
                .AddRoles<TodoRole>()
                .AddEntityFrameworkStores<TodoDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<TodoUser, TodoDbContext>();

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ICsvFileBuilder<TodoItemRecord>, CsvFileBuilder<TodoItemRecord, TodoItemRecordMap>>();

            services.AddTransient<IDateTimeService, DateTimeService>();

            services.AddScoped<IOrchestrator, Orchestrator>();
            services.AddScoped<IUnitOfWork, UnitOfWork<TodoDbContext>>();
            services.AddScoped<IMessages, Messages>();
            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
            
            services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();
            
            // services
            //     .AddSingleton<IEventDeserializer>(
            //         new JsonEventDeserializer(
            //             new[]
            //             {
            //                 typeof(FoobarCreatedEvent).Assembly,
            //                 typeof(CreateFoobar).Assembly
            //             }));
            
            // var kafkaConnStr = configuration.GetConnectionString("kafka");
            // var eventsTopicName = configuration["eventsTopicName"];
            // var groupName = configuration["eventsTopicGroupName"];
            // var consumerConfig = new EventConsumerConfig(kafkaConnStr, eventsTopicName, groupName);
            
            // var eventStoreConnStr = configuration.GetConnectionString("eventstore");
            
            // services.RegisterInfrastructure<TodoDbContext>(eventStoreConnStr, consumerConfig)
            //     .AddEventsRepository<Foobar, Guid>()
            //     .AddEventsService<Foobar, Guid>()
            //     .AddKafkaEventProducer<Foobar, Guid>(consumerConfig);

            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}