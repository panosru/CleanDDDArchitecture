namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.CrossCutting
{
    #region

    using Core.Repositories;
    using Infrastructure.Persistence.Configurations;
    using Infrastructure.Repositories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Todo.Infrastructure.Persistence.Contexts;

    #endregion

    public static class TodoItemDependencyInjectionRegistry
    {
        public static IServiceCollection AddTodoItem(
            this IServiceCollection services,
            IConfiguration          configuration)
        {
            services.AddScoped<ITodoItemRepositoryRead, TodoItemRepositoryRead>();
            services.AddScoped<ITodoItemRepositoryWrite, TodoItemRepositoryWrite>();

            TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoItemConfiguration());

            return services;
        }
    }
}