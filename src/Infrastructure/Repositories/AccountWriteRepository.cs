namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Accounts;
    using Application.Repositories;
    using Aviant.DDD.Domain.Aggregates;
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence.Contexts;

    // public class AccountWriteRepository //TODO: Maybe should add methods for easy creation?
    //     : RepositoryWriteOnly<TodoDbContext, TodoUser, TodoRole, AccountEntity, AccountId>,
    //         IAccountWriteRepository
    // {
    //     public AccountWriteRepository(TodoDbContext context)
    //         : base(context)
    //     {
    //     }
    // }
}