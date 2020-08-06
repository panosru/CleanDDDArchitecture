namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class AccountRead : 
        BaseReadOnly<ApplicationDbContext, ApplicationUser, ApplicationRole, Account, int>, IAccountRead
    {
        public AccountRead(ApplicationDbContext context) : base(context)
        {
        }
    }
}