using System.Reflection;
using AutoMapper;
using Aviant.Application.Mappings;
using CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

namespace CleanDDDArchitecture.Domains.Todo.CrossCutting;

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
        {
            var migrationsAssembly = context.Database.GetService<IMigrationsAssembly>();
            if (migrationsAssembly.Migrations.Count > 0)
                await context.Database.MigrateAsync().ConfigureAwait(false);
            else if (!await HasTodoTablesAsync(context).ConfigureAwait(false))
                await context.Database.GetService<IRelationalDatabaseCreator>().CreateTablesAsync().ConfigureAwait(false);
        }
    }

    private static async Task<bool> HasTodoTablesAsync(TodoDbContextWrite context)
    {
        var connection = context.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;
        if (shouldCloseConnection)
            await connection.OpenAsync().ConfigureAwait(false);

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = 'TodoLists')
                """;

            var result = await command.ExecuteScalarAsync().ConfigureAwait(false);
            return result is true || result is bool boolResult && boolResult;
        }
        finally
        {
            if (shouldCloseConnection)
                await connection.CloseAsync().ConfigureAwait(false);
        }
    }
}
