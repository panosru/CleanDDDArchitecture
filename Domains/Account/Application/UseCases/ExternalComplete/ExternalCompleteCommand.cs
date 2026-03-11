using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalComplete;

internal sealed record ExternalCompleteCommand(
    string Provider,
    string Code,
    string State,
    string RedirectUri) : Command<ExternalAuthenticationResultDto?>
{
    internal sealed class ExternalCompleteCommandHandler
        : CommandHandler<ExternalCompleteCommand, ExternalAuthenticationResultDto?>
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public ExternalCompleteCommandHandler(
            IAccountAuthenticationService authenticationService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        public override async Task<ExternalAuthenticationResultDto?> Handle(
            ExternalCompleteCommand command,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId == Guid.Empty
                ? (Guid?)null
                : _currentUserService.UserId;

            return await _authenticationService
                .CompleteExternalAuthenticationAsync(
                    command.Provider,
                    command.Code,
                    command.State,
                    command.RedirectUri,
                    userId,
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
