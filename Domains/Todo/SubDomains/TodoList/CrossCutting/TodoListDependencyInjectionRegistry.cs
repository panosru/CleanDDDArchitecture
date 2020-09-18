namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.CrossCutting
{
    using Application.UseCases.Create;
    using Application.UseCases.Delete;
    using Application.UseCases.Export;
    using Application.UseCases.GetAll;
    using Aviant.DDD.Infrastructure.CrossCutting;
    using Core.Repositories;
    using Infrastructure.Persistence.Configurations;
    using Infrastructure.Repositories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Todo.Infrastructure.Persistence.Contexts;

    public static class TodoListDependencyInjectionRegistry
    {
        private const string CurrentDomain = "Todo";
        
        private const string CurrentSubDomain = "TodoList";
        
        private static IConfiguration Configuration { get; } =
            DependencyInjectionRegistry.GetDomainConfiguration(
                $"{CurrentDomain}.{CurrentSubDomain}".ToLower());
        
        public static IServiceCollection AddTodoListSubDomain(this IServiceCollection services)
        {
            services.AddScoped<ITodoListRepositoryRead, TodoListRepositoryRead>();
            services.AddScoped<ITodoListRepositoryWrite, TodoListRepositoryWrite>();

            services.AddScoped(typeof(GetAllUseCase));
            services.AddScoped(typeof(CreateTodoListUseCase));
            services.AddScoped(typeof(DeleteTodoListUseCase));
            services.AddScoped(typeof(ExportTodoListUseCase));

            TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoListConfiguration());

            return services;
        }
    }
}