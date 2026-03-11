using Aviant.Application.ApplicationEvents;
using Aviant.Application.Commands;
using Aviant.Application.Identity;
using Aviant.Application.Processors;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount.Events;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;

internal sealed record DeleteAccountRequestCommand(string CurrentPassword) : Command<AccountDeletionRequestResult>
{
    internal sealed class DeleteAccountRequestCommandHandler
        : CommandHandler<DeleteAccountRequestCommand, AccountDeletionRequestResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public DeleteAccountRequestCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<AccountDeletionRequestResult> Handle(
            DeleteAccountRequestCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .RequestAccountDeletionAsync(_currentUserService.UserId, command.CurrentPassword, cancellationToken)
                .ConfigureAwait(false);
    }

    internal sealed class DeleteAccountRequestCommandPostProcessor
        : RequestPostProcessor<DeleteAccountRequestCommand, AccountDeletionRequestResult>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        public DeleteAccountRequestCommandPostProcessor(IApplicationEventDispatcher applicationEventDispatcher) =>
            _applicationEventDispatcher = applicationEventDispatcher;

        public override Task Process(
            DeleteAccountRequestCommand request,
            AccountDeletionRequestResult response,
            CancellationToken cancellationToken)
        {
            if (!response.Result.Succeeded || response.Ticket is null)
                return Task.CompletedTask;

            _applicationEventDispatcher.AddPostCommitEvent(
                new AccountDeletionRequestedApplicationEvent(
                    response.Ticket.Email,
                    response.Ticket.FullName,
                    response.Ticket.Token));

            return Task.CompletedTask;
        }
    }
}
