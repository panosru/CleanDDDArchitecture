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

        public override async Task<AccountUser> Handle(
            ProfileAccountQuery request,
            CancellationToken   cancellationToken)
        {
            var user = await _accountUserManager.FindByIdAsync(CurrentUserService.UserId.ToString()).ConfigureAwait(false);

            return user ?? throw new KeyNotFoundException($"Account '{CurrentUserService.UserId}' was not found.");
        }
    }

    #endregion
}
