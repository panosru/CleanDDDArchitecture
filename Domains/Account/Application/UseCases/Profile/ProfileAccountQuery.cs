using Aviant.Application.Identity;
using Aviant.Application.Queries;
using Aviant.Core.Services;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Profile;

internal sealed record ProfileAccountQuery : Query<AccountUser>
{
    #region Nested type: GetAccountQueryHandler

    internal sealed class GetAccountQueryHandler : QueryHandler<ProfileAccountQuery, AccountUser>
    {
        private readonly UserManager<AccountUser> _accountUserManager;

        public GetAccountQueryHandler(UserManager<AccountUser> accountUserManager) =>
            _accountUserManager = accountUserManager;

        private static ICurrentUserService CurrentUserService =>
            ServiceLocator.ServiceContainer.GetService<ICurrentUserService>(
                typeof(ICurrentUserService));

        public override Task<AccountUser> Handle(
            ProfileAccountQuery request,
            CancellationToken   cancellationToken) =>
            _accountUserManager.FindByIdAsync(CurrentUserService.UserId.ToString());
    }

    #endregion
}
