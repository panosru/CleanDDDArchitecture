namespace CleanDDDArchitecture.Infrastructure
{
    using System.IdentityModel.Tokens.Jwt;
    using Application.Persistence;
    using Application.Repositories;
    using Application.TodoLists.Queries.ExportTodos;
    using Aviant.DDD.Application.Events;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Domain.Events;
    using Aviant.DDD.Domain.Notifications;
    using Aviant.DDD.Domain.Persistence;
    using Aviant.DDD.Domain.Services;
    using Aviant.DDD.Infrastructure.Persistance;
    using Aviant.DDD.Infrastructure.Services;
    using Files.Maps;
    using Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity.UI.Services;
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
            services.AddScoped<IAccountReadRepository, AccountReadRepository>();

            #endregion

            #region Write Repositories

            services.AddScoped<ITodoItemWriteRepository, TodoItemWriteRepository>();
            services.AddScoped<ITodoListWriteRepository, TodoListWriteRepository>();
            services.AddScoped<IAccountWriteRepository, AccountWriteRepository>();

            #endregion

            services.AddDefaultIdentity<TodoUser>()
                .AddRoles<TodoRole>()
                .AddEntityFrameworkStores<TodoDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<TodoUser, TodoDbContext>();

            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ICsvFileBuilder<TodoItemRecord>, CsvFileBuilder<TodoItemRecord, TodoItemRecordMap>>();

            services.AddScoped<IOrchestrator, Orchestrator>();
            services.AddScoped<IUnitOfWork, UnitOfWork<TodoDbContext>>();
            services.AddScoped<INotifications, Notifications>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();

            services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}