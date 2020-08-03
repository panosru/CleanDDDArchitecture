namespace CleanArchitecture.Infrastructure
{
    using Application.Common.Interfaces;
    using Application.TodoLists.Queries.ExportTodos;
    using Aviant.DDD.Application;
    using Aviant.DDD.Infrastructure.Files;
    using Aviant.DDD.Infrastructure.Service;
    using Files.Maps;
    using Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;
    using IIdentityService = Aviant.DDD.Application.Identity.IService;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("CleanArchitectureDb"));
            else
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddTransient<IDateTime, DateTime>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ICsvFileBuilder<TodoItemRecord>, CsvFileBuilder<TodoItemRecord, TodoItemRecordMap>>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}