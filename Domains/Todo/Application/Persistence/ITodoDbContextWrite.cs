namespace CleanDDDArchitecture.Domains.Todo.Application.Persistence;

using Aviant.DDD.Application.Persistance;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

public interface ITodoDbContextWrite : IDbContextWrite
{
    DbSet<TodoListEntity> TodoLists { get; set; }

    DbSet<TodoItemEntity> TodoItems { get; set; }
}