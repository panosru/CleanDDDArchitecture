namespace CleanDDDArchitecture.Domains.Todo.CrossCutting
{
    #region

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
    using SubDomains.TodoItem.Application.UseCases.Create.Dtos;
    using SubDomains.TodoItem.Application.UseCases.Create.Validators;
    using SubDomains.TodoList.Application.UseCases.Create;
    using SubDomains.TodoList.Application.UseCases.Create.Dtos;
    using SubDomains.TodoList.Application.UseCases.Create.Validators;

    #endregion

    public static class TodoCrossCutting
    {
        public static IEnumerable<Profile> AutoMapperProfiles() => new List<Profile>
        {
            new MappingProfile(typeof(TodoItemDto).Assembly),
            new MappingProfile(typeof(TodoListDto).Assembly)
        };

        public static IEnumerable<Assembly> ValidatorAssemblies() => new List<Assembly>
        {
            typeof(CreateTodoItemCommandValidator).Assembly,
            typeof(CreateTodoListCommandValidator).Assembly
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
                await context.Database.MigrateAsync();
        }
    }
}