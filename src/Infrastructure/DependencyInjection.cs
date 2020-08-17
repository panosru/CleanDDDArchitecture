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
    using Aviant.DDD.Infrastructure.Files;
    using Aviant.DDD.Infrastructure.Persistance;
    using Aviant.DDD.Infrastructure.Service;
    using Files.Maps;
    using Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;
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
                services.AddDbContext<ApplicationDbContext>(
                    options =>
                        options.UseInMemoryDatabase("CleanDDDArchitectureDb"));
            else
                services.AddDbContext<ApplicationDbContext>(
                    options =>
                        options.UseNpgsql(
                            configuration.GetConnectionString("DefaultConnection"),
                            b =>
                                b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(
                provider =>
                    provider.GetService<ApplicationDbContext>());

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

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ICsvFileBuilder<TodoItemRecord>, CsvFileBuilder<TodoItemRecord, TodoItemRecordMap>>();

            services.AddScoped<IOrchestrator, Orchestrator>();
            services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            services.AddScoped<INotifications, Notifications>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<Aviant.DDD.Application.Persistance.IApplicationDbContext, ApplicationDbContext>();

            services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();
            
            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}