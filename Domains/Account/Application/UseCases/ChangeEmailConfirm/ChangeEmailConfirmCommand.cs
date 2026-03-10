using Aviant.Application.EventSourcing.Commands;
using Aviant.Application.Identity;
using Aviant.Core.Messages;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailConfirm;

internal sealed record ChangeEmailConfirmCommand(string CurrentEmail, string NewEmail, string Token)
    : Command<AccountAggregate, AccountAggregateId>
{
    private string CurrentEmail { get; } = CurrentEmail;

    private string NewEmail { get; } = NewEmail;

    private string Token { get; } = Token;

    internal sealed class ChangeEmailConfirmCommandHandler
        : CommandHandler<ChangeEmailConfirmCommand, AccountAggregate, AccountAggregateId>
    {
        private readonly IIdentityService _identityService;
        private readonly IMessages _messages;

        public ChangeEmailConfirmCommandHandler(
            IIdentityService identityService,
            IMessages messages)
        {
            _identityService = identityService;
            _messages = messages;
        }

        public override async Task<AccountAggregate> Handle(
            ChangeEmailConfirmCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _identityService
                .ConfirmEmailChangeAsync(command.CurrentEmail, command.NewEmail, command.Token, cancellationToken)
                .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    _messages.AddMessage(error);

                return null!;
            }

            var userId = await _identityService.GetUserIdByEmailAsync(command.NewEmail, cancellationToken)
                .ConfigureAwait(false);

            if (userId is null)
            {
                _messages.AddMessage("User not found.");
                return null!;
            }

            var account = await EventsService
                .RehydrateAsync(new AccountAggregateId(userId.Value), cancellationToken)
                .ConfigureAwait(false);

            if (account is null)
            {
                _messages.AddMessage("Account aggregate not found.");
                return null!;
            }

            account.ChangeEmail(command.NewEmail);

            return account;
        }
    }
}
