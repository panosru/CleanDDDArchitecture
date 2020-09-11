namespace CleanDDDArchitecture.Domains.Todo.Application.Persistence
{
    #region

    using Aviant.DDD.Application.Persistance;
    using Core.Entities;
    using Microsoft.EntityFrameworkCore;

    #endregion

    public interface ITodoDbContextRead : IDbContextRead
    {
        DbSet<TodoListEntity> TodoLists { get; set; }

        DbSet<TodoItemEntity> TodoItems { get; set; }
    }
}