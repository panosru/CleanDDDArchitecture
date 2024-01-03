using CleanDDDArchitecture.Domains.Todo.Application.Persistence;
using Aviant.Application.Persistence;
using Aviant.Application.Persistence.Orchestration;
using Aviant.Application.Services;
using Aviant.Infrastructure.CrossCutting;
using Aviant.Infrastructure.Persistence;
using Aviant.Infrastructure.Services;
using CleanDDDArchitecture.Domains.Todo.Infrastructure;
using CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Files.Maps;

namespace CleanDDDArchitecture.Domains.Todo.CrossCutting;

public static class TodoDependencyInjectionRegistry
{
    private const string CurrentDomain = "Todo";

    private static IConfiguration Configuration { get; } =
        DependencyInjectionRegistry.GetDomainConfiguration(CurrentDomain.ToLower());

    public static IServiceCollection AddTodoDomain(this IServiceCollection services)
    {
        services.AddScoped(_ => new TodoDomainConfiguration(Configuration));

        if (Configuration.GetValue<bool>("UseInMemoryDatabase"))
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
                    Configuration.GetConnectionString("PGSQLConnection"),
                    b =>
                        b.MigrationsAssembly(typeof(TodoDbContextWrite).Assembly.FullName)));

        services.AddScoped<ITodoDbContextWrite>(
            provider =>
                provider.GetService<TodoDbContextWrite>()!);


        services.AddDbContext<TodoDbContextRead>(
            options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("PGSQLConnection"),
                    b =>
                        b.MigrationsAssembly(typeof(TodoDbContextRead).Assembly.FullName)));

        services.AddScoped<ITodoDbContextRead>(
            provider =>
                provider.GetService<TodoDbContextRead>()!);

        services.AddTodoItemSubDomain();
        services.AddTodoListSubDomain();

        services.AddTransient<ICsvFileBuilder<TodoItemRecord>, CsvFileBuilder<TodoItemRecord, TodoItemRecordMap>>();

        services.AddScoped<IUnitOfWork<ITodoDbContextWrite>, UnitOfWork<ITodoDbContextWrite>>();

        services.AddScoped<IOrchestrator<ITodoDbContextWrite>, Orchestrator<ITodoDbContextWrite>>();

        return services;
    }

    public static IHealthChecksBuilder AddTodoChecks(this IHealthChecksBuilder builder) =>
        builder.AddDbContextCheck<TodoDbContextWrite>();
}
