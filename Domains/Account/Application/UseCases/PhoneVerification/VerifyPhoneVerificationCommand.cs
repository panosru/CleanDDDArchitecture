using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;

internal sealed record VerifyPhoneVerificationCommand(string PhoneNumber, string Code, bool IsChange) : Command<IdentityResult>
{
    internal sealed class VerifyPhoneVerificationCommandHandler : CommandHandler<VerifyPhoneVerificationCommand, IdentityResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public VerifyPhoneVerificationCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            VerifyPhoneVerificationCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .VerifyPhoneVerificationAsync(
                    _currentUserService.UserId,
                    command.PhoneNumber,
                    command.Code,
                    command.IsChange,
                    cancellationToken)
                .ConfigureAwait(false);
    }
}
