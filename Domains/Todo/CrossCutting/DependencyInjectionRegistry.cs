namespace CleanDDDArchitecture.Domains.Todo.CrossCutting
{
    using Application.Persistence;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.Persistance;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Infrastructure.Persistence;
    using Aviant.DDD.Infrastructure.Services;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SubDomains.TodoItem.CrossCutting;
    using SubDomains.TodoList.Application.UseCases.Export;
    using SubDomains.TodoList.CrossCutting;
    using SubDomains.TodoList.Infrastructure.Files.Maps;

    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddTodo(
            this IServiceCollection services,
            IConfiguration          configuration)
        {
            
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                // services.AddDbContext<TodoDbContextWrite>(
                //     options =>
                //         options.UseInMemoryDatabase("CleanDDDArchitectureDb"));
                //
                // services.AddDbContext<TodoDbContextRead>(
                //     options =>
                //         options.UseInMemoryDatabase("CleanDDDArchitectureDb"));
            }

            services.AddDbContext<TodoDbContextWrite>(
                options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultWriteConnection"),
                        b =>
                            b.MigrationsAssembly(typeof(TodoDbContextWrite).Assembly.FullName)));


            services.AddScoped<ITodoDbContextWrite>(
                provider =>
                    provider.GetService<TodoDbContextWrite>());
            
            
            services.AddDbContext<TodoDbContextRead>(
                options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultWriteConnection"),
                        b =>
                            b.MigrationsAssembly(typeof(TodoDbContextRead).Assembly.FullName)));

            services.AddScoped<ITodoDbContextRead>(
                provider =>
                    provider.GetService<TodoDbContextRead>());
            
            // services.AddDefaultIdentity<TodoUser>()
            //         .AddRoles<TodoRole>()
            //         .AddEntityFrameworkStores<TodoDbContextWrite>();

            // services.AddIdentityServer()
            //         .AddApiAuthorization<TodoUser, TodoDbContextWrite>();
            
            
            services.AddTodoItem(configuration);
            services.AddTodoList(configuration);

            services.AddTransient<ICsvFileBuilder<TodoItemRecord>, CsvFileBuilder<TodoItemRecord, TodoItemRecordMap>>();

            services.AddScoped<IOrchestrator<TodoDbContextWrite>, Orchestrator<TodoDbContextWrite>>();
            services.AddScoped<IUnitOfWork<TodoDbContextWrite>, UnitOfWork<TodoDbContextWrite>>();
            
            return services;
        }
    }
}