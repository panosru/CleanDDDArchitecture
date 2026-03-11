using Aviant.Application.Identity;
using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.TrustedDevices;

internal sealed record ListTrustedDevicesQuery : Query<IReadOnlyCollection<AccountTrustedDeviceDto>>
{
    internal sealed class ListTrustedDevicesQueryHandler : QueryHandler<ListTrustedDevicesQuery, IReadOnlyCollection<AccountTrustedDeviceDto>>
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public ListTrustedDevicesQueryHandler(
            IAccountAuthenticationService authenticationService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IReadOnlyCollection<AccountTrustedDeviceDto>> Handle(
            ListTrustedDevicesQuery request,
            CancellationToken cancellationToken) =>
            await _authenticationService
                .GetTrustedDevicesAsync(_currentUserService.UserId, cancellationToken)
                .ConfigureAwait(false);
    }
}
