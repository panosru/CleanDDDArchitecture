using Aviant.Application.Commands;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaVerify;

internal sealed record MfaVerifyCommand(string Code) : Command<MfaRecoveryCodesTicket?>
{
    private string Code { get; } = Code;

    internal sealed class MfaVerifyCommandHandler : CommandHandler<MfaVerifyCommand, MfaRecoveryCodesTicket?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public MfaVerifyCommandHandler(
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public override async Task<MfaRecoveryCodesTicket?> Handle(
            MfaVerifyCommand command,
            CancellationToken cancellationToken) =>
            await _identityService
                .EnableMfaAsync(_currentUserService.UserId, command.Code, cancellationToken)
                .ConfigureAwait(false);
    }
}
