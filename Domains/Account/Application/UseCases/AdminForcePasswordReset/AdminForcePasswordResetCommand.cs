using Aviant.Application.ApplicationEvents;
using Aviant.Application.Commands;
using Aviant.Application.Identity;
using Aviant.Application.Processors;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword.Events;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminForcePasswordReset;

internal sealed record AdminForcePasswordResetCommand(Guid AccountId) : Command<PasswordResetTicket?>
{
    internal sealed class AdminForcePasswordResetCommandHandler
        : CommandHandler<AdminForcePasswordResetCommand, PasswordResetTicket?>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminForcePasswordResetCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<PasswordResetTicket?> Handle(
            AdminForcePasswordResetCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .GeneratePasswordResetForUserAsync(_currentUserService.UserId, command.AccountId, cancellationToken)
                .ConfigureAwait(false);
    }

    internal sealed class AdminForcePasswordResetCommandPostProcessor
        : RequestPostProcessor<AdminForcePasswordResetCommand, PasswordResetTicket?>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        public AdminForcePasswordResetCommandPostProcessor(IApplicationEventDispatcher applicationEventDispatcher) =>
            _applicationEventDispatcher = applicationEventDispatcher;

        public override Task Process(
            AdminForcePasswordResetCommand request,
            PasswordResetTicket? response,
            CancellationToken cancellationToken)
        {
            if (response is null)
                return Task.CompletedTask;

            _applicationEventDispatcher.AddPostCommitEvent(
                new PasswordResetRequestedApplicationEvent(
                    response.Email,
                    response.FullName,
                    response.Token));

            return Task.CompletedTask;
        }
    }
}
