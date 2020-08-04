namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class TodoItemWrite : BaseWriteOnly<ApplicationDbContext, ApplicationUser, ApplicationRole, TodoItem, int>
    {
        public TodoItemWrite(ApplicationDbContext context) : base(context)
        {
        }
    }
}