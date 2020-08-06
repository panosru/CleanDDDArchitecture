namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class TodoItemWrite : 
        BaseWriteOnly<ApplicationDbContext, ApplicationUser, ApplicationRole, TodoItem, int>, ITodoItemWrite
    {
        public TodoItemWrite(ApplicationDbContext context) : base(context)
        {
        }
    }
}