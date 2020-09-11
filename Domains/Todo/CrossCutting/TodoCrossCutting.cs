namespace CleanDDDArchitecture.Domains.Todo.CrossCutting
{
    using System.Collections.Generic;
    using System.Reflection;
    using AutoMapper;
    using Aviant.DDD.Application.Mappings;
    using SubDomains.TodoItem.Application.UseCases.Create;
    using SubDomains.TodoItem.Application.UseCases.Create.Dtos;
    using SubDomains.TodoItem.Application.UseCases.Create.Validators;
    using SubDomains.TodoList.Application.UseCases.Create;
    using SubDomains.TodoList.Application.UseCases.Create.Dtos;
    using SubDomains.TodoList.Application.UseCases.Create.Validators;

    public static class TodoCrossCutting
    {
        public static IEnumerable<Profile> TodoAutoMapperProfiles()
        {
            return new List<Profile>
            {
                new MappingProfile(typeof(TodoItemDto).Assembly),
                new MappingProfile(typeof(TodoListDto).Assembly)
            };
        }

        public static IEnumerable<Assembly> TodoValidatorAssemblies()
        {
            return new List<Assembly>
            {
                typeof(CreateTodoItemCommandValidator).Assembly,
                typeof(CreateTodoListCommandValidator).Assembly
            };
        }
        
        public static IEnumerable<Assembly> TodoMediatorAssemblies()
        {
            return new List<Assembly>
            {
                typeof(CreateTodoItemCommand).Assembly,
                typeof(CreateTodoListCommand).Assembly
            };
        }
    }
}