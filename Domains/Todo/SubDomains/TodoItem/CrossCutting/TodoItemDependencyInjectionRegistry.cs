namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.CrossCutting
{
    using Application.UseCases.Create;
    using Application.UseCases.Delete;
    using Application.UseCases.GetBy;
    using Application.UseCases.Update;
    using Application.UseCases.UpdateDetails;
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

            services.AddScoped(typeof(TodoItemCreateUseCase));
            services.AddScoped(typeof(TodoItemDeleteUseCase));
            services.AddScoped(typeof(TodoItemGetByUseCase));
            services.AddScoped(typeof(TodoItemUpdateUseCase));
            services.AddScoped(typeof(TodoItemUpdatedetailsUseCase));

            TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoItemConfiguration());

            return services;
        }
    }
}