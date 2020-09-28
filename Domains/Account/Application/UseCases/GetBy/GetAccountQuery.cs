namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Queries;
    using Identity;
    using Microsoft.AspNetCore.Identity;

    internal sealed class GetAccountQuery : Query<AccountUser>
    {
        public GetAccountQuery(Guid id) => Id = id;

        public Guid Id { get; }
    }

    internal sealed class GetAccountQueryHandler : QueryHandler<GetAccountQuery, AccountUser>
    {
        private readonly UserManager<AccountUser> _accountUserManager;

        public GetAccountQueryHandler(UserManager<AccountUser> accountUserManager) =>
            _accountUserManager = accountUserManager;

        public override Task<AccountUser> Handle(GetAccountQuery request, CancellationToken cancellationToken) =>
            _accountUserManager.FindByIdAsync(request.Id.ToString());
    }
}