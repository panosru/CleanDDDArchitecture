namespace CleanDDDArchitecture.Domains.Todo.CrossCutting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Mappings;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using SubDomains.TodoItem.Application.UseCases.Create;
    using SubDomains.TodoList.Application.UseCases.Create;

    public static class TodoCrossCutting
    {
        private static readonly Assembly TodoItemApplicationAssembly = typeof(TodoItemCreateUseCase).Assembly;

        private static readonly Assembly TodoListApplicationAssembly = typeof(CreateTodoListUseCase).Assembly;

        public static IEnumerable<Profile> AutoMapperProfiles() => new List<Profile>
        {
            new MappingProfile(TodoItemApplicationAssembly),
            new MappingProfile(TodoListApplicationAssembly)
        };

        public static IEnumerable<Assembly> ValidatorAssemblies() => new List<Assembly>
        {
            TodoItemApplicationAssembly,
            TodoListApplicationAssembly
        };

        public static IEnumerable<Assembly> MediatorAssemblies() => new List<Assembly>
        {
            TodoItemApplicationAssembly,
            TodoListApplicationAssembly
        };

        public static async Task GenerateTodoMigrationsIfNewExists(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<TodoDbContextWrite>();

            if (context.Database.IsNpgsql())
                await context.Database.MigrateAsync().ConfigureAwait(false);
        }
    }
}
