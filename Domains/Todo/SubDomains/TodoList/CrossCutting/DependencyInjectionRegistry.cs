namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.CrossCutting
{
    using Core.Repositories;
    using Infrastructure.Persistence.Configurations;
    using Infrastructure.Repositories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Todo.Infrastructure.Persistence.Contexts;

    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddTodoList(
            this IServiceCollection services,
            IConfiguration          configuration)
        {

            services.AddScoped<ITodoListRepositoryRead, TodoListRepositoryRead>();
            services.AddScoped<ITodoListRepositoryWrite, TodoListRepositoryWrite>();
            
            TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoListConfiguration());

            return services;
        }
    }
}