using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy;

internal sealed record GetAccountQuery(Guid Id) : Query<AccountUser>
{
    private Guid Id { get; } = Id;

    #region Nested type: GetAccountQueryHandler

    internal sealed class GetAccountQueryHandler : QueryHandler<GetAccountQuery, AccountUser>
    {
        private readonly UserManager<AccountUser> _accountUserManager;

        public GetAccountQueryHandler(UserManager<AccountUser> accountUserManager) =>
            _accountUserManager = accountUserManager;

        public override Task<AccountUser> Handle(GetAccountQuery request, CancellationToken cancellationToken) =>
            _accountUserManager.FindByIdAsync(request.Id.ToString());
    }

    #endregion
}
