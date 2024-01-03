using Aviant.Application.Persistence;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanDDDArchitecture.Domains.Todo.Application.Persistence;

public interface ITodoDbContextRead : IDbContextRead
{
    DbSet<TodoListEntity> TodoLists { get; set; }

    DbSet<TodoItemEntity> TodoItems { get; set; }
}
