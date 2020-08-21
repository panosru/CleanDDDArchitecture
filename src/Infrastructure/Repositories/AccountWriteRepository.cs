namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence.Contexts;

    public class AccountWriteRepository //TODO: Maybe should add methods for easy creation?
        : RepositoryWriteOnly<TodoDbContext, TodoUser, TodoRole, AccountEntity, int>,
            IAccountWriteRepository
    {
        public AccountWriteRepository(TodoDbContext context)
            : base(context)
        {
        }
    }
}