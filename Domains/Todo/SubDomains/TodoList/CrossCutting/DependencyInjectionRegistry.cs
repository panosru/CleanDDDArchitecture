namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.CrossCutting
{
    using Core.Repositories;
    using Infrastructure.Repositories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddTodoList(
            this IServiceCollection services,
            IConfiguration          configuration)
        {

            services.AddScoped<ITodoListRepositoryRead, TodoListRepositoryRead>();
            services.AddScoped<ITodoListRepositoryWrite, TodoListRepositoryWrite>();

            return services;
        }
    }
}