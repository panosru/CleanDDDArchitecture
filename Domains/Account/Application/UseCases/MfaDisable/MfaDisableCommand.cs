using Aviant.Application.Commands;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaDisable;

internal sealed record MfaDisableCommand(string CurrentPassword) : Command<IdentityResult>
{
    private string CurrentPassword { get; } = CurrentPassword;

    internal sealed class MfaDisableCommandHandler : CommandHandler<MfaDisableCommand, IdentityResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public MfaDisableCommandHandler(
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            MfaDisableCommand command,
            CancellationToken cancellationToken) =>
            await _identityService
                .DisableMfaAsync(_currentUserService.UserId, command.CurrentPassword, cancellationToken)
                .ConfigureAwait(false);
    }
}
