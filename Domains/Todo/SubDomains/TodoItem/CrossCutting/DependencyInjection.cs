namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.CrossCutting
{
    using Core.Repositories;
    using Infrastructure.Repositories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddTodoItem(
            this IServiceCollection services,
            IConfiguration          configuration)
        {

            services.AddScoped<ITodoItemRepositoryRead, TodoItemRepositoryRead>();
            services.AddScoped<ITodoItemRepositoryWrite, TodoItemRepositoryWrite>();
            
            return services;
        }
    }
}