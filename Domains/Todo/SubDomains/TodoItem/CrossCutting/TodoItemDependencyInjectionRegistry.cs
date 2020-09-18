namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.CrossCutting
{
    using Core.Repositories;
    using Infrastructure.Persistence.Configurations;
    using Infrastructure.Repositories;
    using Microsoft.Extensions.DependencyInjection;
    using Todo.Infrastructure.Persistence.Contexts;

    public static class TodoItemDependencyInjectionRegistry
    {
        public static IServiceCollection AddTodoItemSubDomain(this IServiceCollection services)
        {
            services.AddScoped<ITodoItemRepositoryRead, TodoItemRepositoryRead>();
            services.AddScoped<ITodoItemRepositoryWrite, TodoItemRepositoryWrite>();

            TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoItemConfiguration());

            return services;
        }
    }
}