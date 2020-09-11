﻿namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts
{
    using Application.Persistence;
    using Aviant.DDD.Infrastructure.Persistence.Contexts;
    using Core.Entities;
    using Microsoft.EntityFrameworkCore;

    public class TodoDbContextWrite
        : DbContextWrite<TodoDbContextWrite>, ITodoDbContextWrite
    {
        public TodoDbContextWrite(DbContextOptions<TodoDbContextWrite> options)
            : base(options)
        {}

        #region ITodoDbContextWrite Members

        public DbSet<TodoListEntity> TodoLists { get; set; }

        public DbSet<TodoItemEntity> TodoItems { get; set; }

        #endregion
    }
}