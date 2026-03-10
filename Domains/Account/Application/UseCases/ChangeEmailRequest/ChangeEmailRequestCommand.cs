using Aviant.Application.ApplicationEvents;
using Aviant.Application.Commands;
using Aviant.Application.Identity;
using Aviant.Application.Processors;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest.Events;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest;

internal sealed record ChangeEmailRequestCommand(string NewEmail) : Command<EmailChangeTicket?>
{
    private string NewEmail { get; } = NewEmail;

    internal sealed class ChangeEmailRequestCommandHandler : CommandHandler<ChangeEmailRequestCommand, EmailChangeTicket?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public ChangeEmailRequestCommandHandler(
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public override async Task<EmailChangeTicket?> Handle(
            ChangeEmailRequestCommand command,
            CancellationToken cancellationToken) =>
            await _identityService
                .GenerateEmailChangeAsync(_currentUserService.UserId, command.NewEmail, cancellationToken)
                .ConfigureAwait(false);
    }

    internal sealed class ChangeEmailRequestCommandPostProcessor
        : RequestPostProcessor<ChangeEmailRequestCommand, EmailChangeTicket?>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        public ChangeEmailRequestCommandPostProcessor(IApplicationEventDispatcher applicationEventDispatcher) =>
            _applicationEventDispatcher = applicationEventDispatcher;

        public override Task Process(
            ChangeEmailRequestCommand request,
            EmailChangeTicket? response,
            CancellationToken cancellationToken)
        {
            if (response is null)
                return Task.CompletedTask;

            _applicationEventDispatcher.AddPostCommitEvent(
                new EmailChangeRequestedApplicationEvent(
                    response.CurrentEmail,
                    response.NewEmail,
                    response.FullName,
                    response.Token));

            return Task.CompletedTask;
        }
    }
}
