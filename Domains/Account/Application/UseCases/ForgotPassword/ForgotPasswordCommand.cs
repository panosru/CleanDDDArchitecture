using Aviant.Application.ApplicationEvents;
using Aviant.Application.Commands;
using Aviant.Application.Identity;
using Aviant.Application.Processors;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword.Events;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword;

internal sealed record ForgotPasswordCommand(string Email) : Command<PasswordResetTicket?>
{
    private string Email { get; } = Email;

    internal sealed class ForgotPasswordCommandHandler : CommandHandler<ForgotPasswordCommand, PasswordResetTicket?>
    {
        private readonly IIdentityService _identityService;

        public ForgotPasswordCommandHandler(IIdentityService identityService) => _identityService = identityService;

        public override async Task<PasswordResetTicket?> Handle(
            ForgotPasswordCommand command,
            CancellationToken cancellationToken) =>
            await _identityService.GeneratePasswordResetAsync(command.Email, cancellationToken).ConfigureAwait(false);
    }

    internal sealed class ForgotPasswordCommandPostProcessor
        : RequestPostProcessor<ForgotPasswordCommand, PasswordResetTicket?>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        public ForgotPasswordCommandPostProcessor(IApplicationEventDispatcher applicationEventDispatcher) =>
            _applicationEventDispatcher = applicationEventDispatcher;

        public override Task Process(
            ForgotPasswordCommand request,
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
