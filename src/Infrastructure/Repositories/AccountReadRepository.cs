namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence.Contexts;

    public class AccountReadRepository
        : RepositoryReadOnly<TodoDbContextReadOnly, TodoUser, TodoRole, AccountEntity, int>,
            IAccountReadRepository
    {
        public AccountReadRepository(TodoDbContextReadOnly context)
            : base(context)
        {
        }
    }
}