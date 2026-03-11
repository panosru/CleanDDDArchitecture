using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalBegin;

internal sealed record ExternalBeginCommand(string Provider, string RedirectUri)
    : Command<ExternalAuthenticationStartDto?>
{
    internal sealed class ExternalBeginCommandHandler
        : CommandHandler<ExternalBeginCommand, ExternalAuthenticationStartDto?>
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public ExternalBeginCommandHandler(
            IAccountAuthenticationService authenticationService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        public override async Task<ExternalAuthenticationStartDto?> Handle(
            ExternalBeginCommand command,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId == Guid.Empty
                ? (Guid?)null
                : _currentUserService.UserId;

            return await _authenticationService
                .BeginExternalAuthenticationAsync(command.Provider, command.RedirectUri, userId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
