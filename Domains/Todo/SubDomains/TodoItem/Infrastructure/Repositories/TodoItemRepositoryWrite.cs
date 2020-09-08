namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Repositories
{
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Core.Repositories;
    using Todo.Core.Entities;
    using Todo.Infrastructure.Identity;
    using Todo.Infrastructure.Persistence.Contexts;

    public class TodoItemRepositoryWrite //TODO: Add MarkComplete & UnmarkComplete methods?
        : RepositoryWriteOnly<
              TodoDbContextWrite,
              TodoUser,
              TodoRole,
              TodoItemEntity,
              int>,
          ITodoItemRepositoryWrite
    {
        public TodoItemRepositoryWrite(TodoDbContextWrite context)
            :
            base(context)
        { }
    }
}