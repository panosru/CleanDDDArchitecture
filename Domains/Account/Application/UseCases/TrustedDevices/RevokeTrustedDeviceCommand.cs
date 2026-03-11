using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.TrustedDevices;

internal sealed record RevokeTrustedDeviceCommand(Guid TrustedDeviceId) : Command<bool>
{
    internal sealed class RevokeTrustedDeviceCommandHandler : CommandHandler<RevokeTrustedDeviceCommand, bool>
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public RevokeTrustedDeviceCommandHandler(
            IAccountAuthenticationService authenticationService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        public override async Task<bool> Handle(
            RevokeTrustedDeviceCommand command,
            CancellationToken cancellationToken) =>
            await _authenticationService
                .RevokeTrustedDeviceAsync(_currentUserService.UserId, command.TrustedDeviceId, cancellationToken)
                .ConfigureAwait(false);
    }
}
