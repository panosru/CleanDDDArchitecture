namespace CleanDDDArchitecture.Domains.Todo.CrossCutting;

using System.Reflection;
using AutoMapper;
using Aviant.Foundation.Application.Mappings;
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

    public static async Task GenerateTodoMigrationsIfNewExistsAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<TodoDbContextWrite>();

        if (context.Database.IsNpgsql())
            await context.Database.MigrateAsync().ConfigureAwait(false);
    }
}
