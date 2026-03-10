using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Deactivate;

internal sealed record DeactivateCommand(string CurrentPassword) : Command<IdentityResult>
{
    private string CurrentPassword { get; } = CurrentPassword;

    internal sealed class DeactivateCommandHandler : CommandHandler<DeactivateCommand, IdentityResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public DeactivateCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            DeactivateCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .DeactivateAsync(_currentUserService.UserId, command.CurrentPassword, cancellationToken)
                .ConfigureAwait(false);
    }
}
