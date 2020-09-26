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
        public static IEnumerable<Profile> AutoMapperProfiles() => new List<Profile>
        {
            new MappingProfile(typeof(TodoItemViewModel).Assembly),
            new MappingProfile(typeof(CreatedTodoListViewModel).Assembly)
        };

        public static IEnumerable<Assembly> ValidatorAssemblies() => new List<Assembly>
        {
            typeof(CreateTodoItemCommandValidator).Assembly,
            typeof(CreateTodoListInputValidator).Assembly
        };

        public static IEnumerable<Assembly> MediatorAssemblies() => new List<Assembly>
        {
            typeof(CreateTodoItemCommand).Assembly,
            typeof(CreateTodoListCommand).Assembly
        };

        public static async Task GenerateTodoMigrationsIfNewExists(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<TodoDbContextWrite>();

            if (context.Database.IsNpgsql())
                await context.Database.MigrateAsync().ConfigureAwait(false);
        }
    }
}