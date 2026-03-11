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
using System.Globalization;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.CrossCutting;

public static class TodoListDependencyInjectionRegistry
{
    private const string CurrentDomain = "Todo";

    private const string CurrentSubDomain = "TodoList";

    static TodoListDependencyInjectionRegistry() => Configuration =
        DependencyInjectionRegistry.GetDomainConfiguration(
            $"{CurrentDomain}.{CurrentSubDomain}".ToLower(CultureInfo.InvariantCulture));

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private static IConfiguration Configuration { get; }

    public static IServiceCollection AddTodoListSubDomain(this IServiceCollection services)
    {
        services.AddScoped(_ => new TodoListDomainConfiguration(Configuration));

        services.AddScoped<ITodoListRepositoryRead, TodoListRepositoryRead>();
        services.AddScoped<ITodoListRepositoryWrite, TodoListRepositoryWrite>();

        services.AddScoped<GetAllUseCase>();
        services.AddScoped<CreateTodoListUseCase>();
        services.AddScoped<UpdateTodoListUseCase>();
        services.AddScoped<DeleteTodoListUseCase>();
        services.AddScoped<ExportTodoListUseCase>();

        TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoListConfiguration());

        return services;
    }
}
