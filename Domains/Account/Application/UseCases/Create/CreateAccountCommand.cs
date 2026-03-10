using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Aviant.Application.ApplicationEvents;
using Aviant.Core.Messages;
using Aviant.Application.EventSourcing.Commands;
using Aviant.Application.Identity;
using Aviant.Application.Processors;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;

public sealed record CreateAccountCommand(
    string              Password,
    string              FirstName,
    string              LastName,
    string              Email,
    IEnumerable<string> Roles,
    bool                EmailConfirmed) : Command<AccountAggregate, AccountAggregateId>
{
    private string Password { get; } = Password;

    private string FirstName { get; } = FirstName;

    private string LastName { get; } = LastName;

    private string Email { get; } = Email;

    private IEnumerable<string> Roles { get; } = Roles;

    private bool EmailConfirmed { get; } = EmailConfirmed;

    #region Nested type: CreateAccountHandler

    public sealed class CreateAccountHandler
        : CommandHandler<CreateAccountCommand, AccountAggregate, AccountAggregateId>
    {
        private readonly IIdentityService _identityService;
        private readonly IMessages _messages;

        public CreateAccountHandler(
            IIdentityService identityService,
            IMessages messages)
        {
            _identityService = identityService;
            _messages = messages;
        }

        public override async Task<AccountAggregate> Handle(
            CreateAccountCommand command,
            CancellationToken cancellationToken)
        {
            var createUserResult = await _identityService.CreateUserAsync(
                    command.Email,
                    command.Password,
                    command.FirstName,
                    command.LastName,
                    command.Roles,
                    command.EmailConfirmed,
                    cancellationToken)
                .ConfigureAwait(false);

            if (!createUserResult.Result.Succeeded)
            {
                foreach (var error in createUserResult.Result.Errors)
                    _messages.AddMessage(error);

                return null!;
            }

            return AccountAggregate.Create(
                createUserResult.UserId,
                command.Email,
                command.FirstName,
                command.LastName,
                command.Email,
                command.Roles,
                command.EmailConfirmed);
        }
    }

    internal sealed class CreateAccountCommandPostProcessor
        : RequestPostProcessor<CreateAccountCommand, AccountAggregate>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        public CreateAccountCommandPostProcessor(IApplicationEventDispatcher applicationEventDispatcher) =>
            _applicationEventDispatcher = applicationEventDispatcher;

        public override Task Process(
            CreateAccountCommand request,
            AccountAggregate response,
            CancellationToken cancellationToken)
        {
            if (response is null)
                return Task.CompletedTask;

            _applicationEventDispatcher.AddPostCommitEvent(
                new AccountCreatedApplicationEvent(response.Email, response.EmailConfirmed));

            return Task.CompletedTask;
        }
    }

    #endregion
}
