namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.CrossCutting
{
    using Application.UseCases.Create;
    using Application.UseCases.Delete;
    using Application.UseCases.Export;
    using Application.UseCases.GetAll;
    using Application.UseCases.Update;
    using Aviant.DDD.Infrastructure.CrossCutting;
    using Core.Repositories;
    using Infrastructure;
    using Infrastructure.Persistence.Configurations;
    using Infrastructure.Repositories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Todo.Infrastructure.Persistence.Contexts;

    public static class TodoListDependencyInjectionRegistry
    {
        private const string CurrentDomain = "Todo";

        private const string CurrentSubDomain = "TodoList";

        static TodoListDependencyInjectionRegistry() => Configuration =
            DependencyInjectionRegistry.GetDomainConfiguration(
                $"{CurrentDomain}.{CurrentSubDomain}".ToLower());

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private static IConfiguration Configuration { get; }

        public static IServiceCollection AddTodoListSubDomain(this IServiceCollection services)
        {
            services.AddScoped(_ => new TodoListDomainConfiguration(Configuration));

            services.AddScoped<ITodoListRepositoryRead, TodoListRepositoryRead>();
            services.AddScoped<ITodoListRepositoryWrite, TodoListRepositoryWrite>();

            services.AddScoped(typeof(GetAllUseCase));
            services.AddScoped(typeof(CreateTodoListUseCase));
            services.AddScoped(typeof(UpdateTodoListUseCase));
            services.AddScoped(typeof(DeleteTodoListUseCase));
            services.AddScoped(typeof(ExportTodoListUseCase));

            TodoDbContextWrite.AddConfigurationAssemblyFromEntity(new TodoListConfiguration());

            return services;
        }
    }
}
