namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class TodoListWrite :
        BaseWriteOnly<ApplicationDbContext, ApplicationUser, ApplicationRole, TodoList, int>, ITodoListWrite
    {
        public TodoListWrite(ApplicationDbContext context) : base(context)
        {
        }
    }
}