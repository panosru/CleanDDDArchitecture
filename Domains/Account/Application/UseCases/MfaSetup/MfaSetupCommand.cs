using Aviant.Application.Commands;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaSetup;

internal sealed record MfaSetupCommand() : Command<MfaSetupTicket?>
{
    internal sealed class MfaSetupCommandHandler : CommandHandler<MfaSetupCommand, MfaSetupTicket?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public MfaSetupCommandHandler(
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public override async Task<MfaSetupTicket?> Handle(
            MfaSetupCommand command,
            CancellationToken cancellationToken) =>
            await _identityService
                .BeginMfaSetupAsync(_currentUserService.UserId, cancellationToken)
                .ConfigureAwait(false);
    }
}
