using Aviant.Application.ApplicationEvents;
using Aviant.Application.Commands;
using Aviant.Application.Identity;
using Aviant.Application.Processors;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation.Events;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation;

internal sealed record ResendConfirmationCommand(string Email) : Command<EmailConfirmationTicket?>
{
    private string Email { get; } = Email;

    internal sealed class ResendConfirmationCommandHandler : CommandHandler<ResendConfirmationCommand, EmailConfirmationTicket?>
    {
        private readonly IIdentityService _identityService;

        public ResendConfirmationCommandHandler(IIdentityService identityService) => _identityService = identityService;

        public override async Task<EmailConfirmationTicket?> Handle(
            ResendConfirmationCommand command,
            CancellationToken cancellationToken) =>
            await _identityService.GenerateEmailConfirmationAsync(command.Email, cancellationToken).ConfigureAwait(false);
    }

    internal sealed class ResendConfirmationCommandPostProcessor
        : RequestPostProcessor<ResendConfirmationCommand, EmailConfirmationTicket?>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        public ResendConfirmationCommandPostProcessor(IApplicationEventDispatcher applicationEventDispatcher) =>
            _applicationEventDispatcher = applicationEventDispatcher;

        public override Task Process(
            ResendConfirmationCommand request,
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
