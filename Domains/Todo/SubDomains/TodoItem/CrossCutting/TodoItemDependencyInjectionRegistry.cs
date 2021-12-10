namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.CrossCutting;

using Application.UseCases.Create;
using Application.UseCases.Delete;
using Application.UseCases.GetBy;
using Application.UseCases.Update;
using Application.UseCases.UpdateDetails;
using Aviant.DDD.Infrastructure.CrossCutting;
using Core.Repositories;
using Infrastructure;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Infrastructure.Persistence.Contexts;

public static class TodoItemDependencyInjectionRegistry
{
    private const string CurrentDomain = "Todo";

    private const string CurrentSubDomain = "TodoItem";

    static TodoItemDependencyInjectionRegistry() => Configuration =
        DependencyInjectionRegistry.GetDomainConfiguration(
            $"{CurrentDomain}.{CurrentSubDomain}".ToLower());

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private static IConfiguration Configuration { get; }

    public static IServiceCollection AddTodoItemSubDomain(this IServiceCollection services)
    {
        services.AddScoped(_ => new TodoItemDomainConfiguration(Configuration));

        services.AddScoped<ITodoItemRepositoryRead, TodoItemRepositoryRead>();
        services.AddScoped<ITodoItemRepositoryWrite, TodoItemRepositoryWrite>();

        services.AddScoped(typeof(TodoItemCreateUseCase));
        services.AddScoped(typeof(TodoItemDeleteUseCase));
        services.AddScoped(typeof(TodoItemGetByUseCase));
        services.AddScoped(typeof(TodoItemUpdateUseCase));
        services.AddScoped(typeof(TodoItemUpdatedetailsUseCase));

        TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoItemConfiguration());

        return services;
    }
}