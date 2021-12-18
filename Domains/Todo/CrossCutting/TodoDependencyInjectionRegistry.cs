namespace CleanDDDArchitecture.Domains.Todo.CrossCutting;

using Application.Persistence;
using Aviant.DDD.Application.Orchestration;
using Aviant.DDD.Application.Persistence;
using Aviant.DDD.Application.Services;
using Aviant.DDD.Infrastructure.CrossCutting;
using Aviant.DDD.Infrastructure.Persistence;
using Aviant.DDD.Infrastructure.Services;
using Infrastructure;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubDomains.TodoItem.CrossCutting;
using SubDomains.TodoList.Application.UseCases.Export;
using SubDomains.TodoList.CrossCutting;
using SubDomains.TodoList.Infrastructure.Files.Maps;

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
                    Configuration.GetConnectionString("DefaultWriteConnection"),
                    b =>
                        b.MigrationsAssembly(typeof(TodoDbContextWrite).Assembly.FullName)));

        services.AddScoped<ITodoDbContextWrite>(
            provider =>
                provider.GetService<TodoDbContextWrite>()!);


        services.AddDbContext<TodoDbContextRead>(
            options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultWriteConnection"),
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
