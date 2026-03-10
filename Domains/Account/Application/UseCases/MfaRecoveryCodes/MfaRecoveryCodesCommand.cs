using Aviant.Application.Commands;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaRecoveryCodes;

internal sealed record MfaRecoveryCodesCommand(string CurrentPassword) : Command<MfaRecoveryCodesTicket?>
{
    private string CurrentPassword { get; } = CurrentPassword;

    internal sealed class MfaRecoveryCodesCommandHandler : CommandHandler<MfaRecoveryCodesCommand, MfaRecoveryCodesTicket?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public MfaRecoveryCodesCommandHandler(
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public override async Task<MfaRecoveryCodesTicket?> Handle(
            MfaRecoveryCodesCommand command,
            CancellationToken cancellationToken) =>
            await _identityService
                .RegenerateRecoveryCodesAsync(_currentUserService.UserId, command.CurrentPassword, cancellationToken)
                .ConfigureAwait(false);
    }
}
