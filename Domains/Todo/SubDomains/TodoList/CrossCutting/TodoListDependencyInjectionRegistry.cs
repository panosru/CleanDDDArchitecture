using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update;
using Aviant.Infrastructure.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Persistence.Configurations;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.CrossCutting;

public static class TodoListDependencyInjectionRegistry
{
    private const string CurrentDomain = "Todo";

    private const string CurrentSubDomain = "TodoList";

    static TodoListDependencyInjectionRegistry() => Configuration =
        DependencyInjectionRegistry.GetDomainConfiguration(
            $"{CurrentDomain}.{CurrentSubDomain}".ToLower());

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private static IConfiguration Configuration { get; }

    public static IServiceCollection AddTodoListSubDomain(this IServiceCollection services)
    {
        services.AddScoped(_ => new TodoListDomainConfiguration(Configuration));

        services.AddScoped<ITodoListRepositoryRead, TodoListRepositoryRead>();
        services.AddScoped<ITodoListRepositoryWrite, TodoListRepositoryWrite>();

        services.AddScoped(typeof(GetAllUseCase));
        services.AddScoped(typeof(CreateTodoListUseCase));
        services.AddScoped(typeof(UpdateTodoListUseCase));
        services.AddScoped(typeof(DeleteTodoListUseCase));
        services.AddScoped(typeof(ExportTodoListUseCase));

        TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoListConfiguration());

        return services;
    }
}
