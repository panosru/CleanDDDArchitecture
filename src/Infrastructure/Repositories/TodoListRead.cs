namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class TodoListRead :
        BaseReadOnly<ApplicationDbContext, ApplicationUser, ApplicationRole, TodoList, int>, ITodoListRead
    {
        public TodoListRead(ApplicationDbContext context) : base(context)
        {
        }
    }
}