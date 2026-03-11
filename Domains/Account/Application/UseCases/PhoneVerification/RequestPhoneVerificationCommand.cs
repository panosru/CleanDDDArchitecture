using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;

internal sealed record RequestPhoneVerificationCommand(string PhoneNumber, bool IsChange) : Command<IdentityResult>
{
    internal sealed class RequestPhoneVerificationCommandHandler : CommandHandler<RequestPhoneVerificationCommand, IdentityResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public RequestPhoneVerificationCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            RequestPhoneVerificationCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .RequestPhoneVerificationAsync(
                    _currentUserService.UserId,
                    command.PhoneNumber,
                    command.IsChange,
                    cancellationToken)
                .ConfigureAwait(false);
    }
}
