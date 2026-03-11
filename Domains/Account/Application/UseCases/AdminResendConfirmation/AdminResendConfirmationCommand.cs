using Aviant.Application.ApplicationEvents;
using Aviant.Application.Commands;
using Aviant.Application.Identity;
using Aviant.Application.Processors;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation.Events;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminResendConfirmation;

internal sealed record AdminResendConfirmationCommand(Guid AccountId) : Command<EmailConfirmationTicket?>
{
    internal sealed class AdminResendConfirmationCommandHandler
        : CommandHandler<AdminResendConfirmationCommand, EmailConfirmationTicket?>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminResendConfirmationCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<EmailConfirmationTicket?> Handle(
            AdminResendConfirmationCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .GenerateEmailConfirmationForUserAsync(_currentUserService.UserId, command.AccountId, cancellationToken)
                .ConfigureAwait(false);
    }

    internal sealed class AdminResendConfirmationCommandPostProcessor
        : RequestPostProcessor<AdminResendConfirmationCommand, EmailConfirmationTicket?>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        public AdminResendConfirmationCommandPostProcessor(IApplicationEventDispatcher applicationEventDispatcher) =>
            _applicationEventDispatcher = applicationEventDispatcher;

        public override Task Process(
            AdminResendConfirmationCommand request,
            EmailConfirmationTicket? response,
            CancellationToken cancellationToken)
        {
            if (response is null)
                return Task.CompletedTask;

            _applicationEventDispatcher.AddPostCommitEvent(
                new ConfirmationEmailRequestedApplicationEvent(
                    response.Email,
                    response.FullName,
                    response.Token));

            return Task.CompletedTask;
        }
    }
}
