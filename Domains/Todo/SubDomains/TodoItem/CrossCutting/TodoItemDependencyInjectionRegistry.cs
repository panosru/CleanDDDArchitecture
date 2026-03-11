using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails;
using Aviant.Infrastructure.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Persistence.Configurations;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;
using System.Globalization;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.CrossCutting;

public static class TodoItemDependencyInjectionRegistry
{
    private const string CurrentDomain = "Todo";

    private const string CurrentSubDomain = "TodoItem";

    static TodoItemDependencyInjectionRegistry() => Configuration =
        DependencyInjectionRegistry.GetDomainConfiguration(
            $"{CurrentDomain}.{CurrentSubDomain}".ToLower(CultureInfo.InvariantCulture));

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private static IConfiguration Configuration { get; }

    public static IServiceCollection AddTodoItemSubDomain(this IServiceCollection services)
    {
        services.AddScoped(_ => new TodoItemDomainConfiguration(Configuration));

        services.AddScoped<ITodoItemRepositoryRead, TodoItemRepositoryRead>();
        services.AddScoped<ITodoItemRepositoryWrite, TodoItemRepositoryWrite>();

        services.AddScoped<TodoItemCreateUseCase>();
        services.AddScoped<TodoItemDeleteUseCase>();
        services.AddScoped<TodoItemGetByUseCase>();
        services.AddScoped<TodoItemUpdateUseCase>();
        services.AddScoped<TodoItemUpdatedetailsUseCase>();

        TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoItemConfiguration());

        return services;
    }
}
