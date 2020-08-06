namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class TodoItemRead : 
        BaseReadOnly<ApplicationDbContext, ApplicationUser, ApplicationRole, TodoItem, int>, ITodoItemRead
    {
        public TodoItemRead(ApplicationDbContext context) : base(context)
        {
        }
    }
}