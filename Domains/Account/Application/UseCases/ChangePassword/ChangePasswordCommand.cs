using Aviant.Application.Commands;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangePassword;

internal sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : Command<IdentityResult>
{
    private string CurrentPassword { get; } = CurrentPassword;

    private string NewPassword { get; } = NewPassword;

    internal sealed class ChangePasswordCommandHandler : CommandHandler<ChangePasswordCommand, IdentityResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public ChangePasswordCommandHandler(
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            ChangePasswordCommand command,
            CancellationToken cancellationToken) =>
            await _identityService
                .ChangePasswordAsync(
                    _currentUserService.UserId,
                    command.CurrentPassword,
                    command.NewPassword,
                    cancellationToken)
                .ConfigureAwait(false);
    }
}
