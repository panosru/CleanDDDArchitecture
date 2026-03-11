using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalUnlink;

internal sealed record ExternalUnlinkCommand(string Provider) : Command<Aviant.Application.Identity.IdentityResult>
{
    internal sealed class ExternalUnlinkCommandHandler
        : CommandHandler<ExternalUnlinkCommand, Aviant.Application.Identity.IdentityResult>
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public ExternalUnlinkCommandHandler(
            IAccountAuthenticationService authenticationService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        public override async Task<Aviant.Application.Identity.IdentityResult> Handle(
            ExternalUnlinkCommand command,
            CancellationToken cancellationToken)
        {
            return await _authenticationService
                .UnlinkExternalLoginAsync(_currentUserService.UserId, command.Provider, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
