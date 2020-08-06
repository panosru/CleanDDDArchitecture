namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class AccountWrite :
        BaseWriteOnly<ApplicationDbContext, ApplicationUser, ApplicationRole, Account, int>, IAccountWrite
    {
        public AccountWrite(ApplicationDbContext context) : base(context)
        {
        }
    }
}