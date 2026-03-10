using System.Text;
using Aviant.Application.Identity;
using Aviant.Application.EventSourcing.Commands;
using Aviant.Core.Messages;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Microsoft.AspNetCore.WebUtilities;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;

internal sealed record ConfirmEmailCommand(string Token, string Email) : Command<AccountAggregate, AccountAggregateId>
{
    private string Token { get; } = Token;

    private string Email { get; } = Email;

    #region Nested type: ConfirmEmailCommandHandler

    internal sealed class ConfirmEmailCommandHandler
        : CommandHandler<ConfirmEmailCommand, AccountAggregate, AccountAggregateId>
    {
        private readonly IIdentityService _identityIdentityService;
        private readonly IMessages _messages;

        public ConfirmEmailCommandHandler(
            IIdentityService identityIdentityService,
            IMessages messages)
        {
            _identityIdentityService = identityIdentityService;
            _messages = messages;
        }

        public override async Task<AccountAggregate> Handle(
            ConfirmEmailCommand command,
            CancellationToken   cancellationToken)
        {
            string token;
            try
            {
                token = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(command.Token));
            }
            catch (FormatException)
            {
                _messages.AddMessage("Invalid email confirmation token.");
                return null!;
            }

            var result = await _identityIdentityService.ConfirmEmailAsync(
                    token,
                    command.Email,
                    cancellationToken)
               .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    _messages.AddMessage(error);

                return null!;
            }

            var userId = await _identityIdentityService.GetUserIdByEmailAsync(
                    command.Email,
                    cancellationToken)
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

            account.ConfirmEmail();

            return account;
        }
    }

    #endregion
}
