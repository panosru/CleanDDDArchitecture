using System.Text;
using System.Web;
using Aviant.Application.Commands;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;

internal sealed record ConfirmEmailCommand(string Token, string Email) : Command<IdentityResult>
{
    private string Token { get; } = Token;

    private string Email { get; } = Email;

    #region Nested type: ConfirmEmailCommandHandler

    internal sealed class ConfirmEmailCommandHandler
        : CommandHandler<ConfirmEmailCommand, IdentityResult>
    {
        private readonly IIdentityService _identityIdentityService;

        public ConfirmEmailCommandHandler(IIdentityService identityIdentityService) =>
            _identityIdentityService = identityIdentityService;

        public override async Task<IdentityResult> Handle(
            ConfirmEmailCommand command,
            CancellationToken   cancellationToken)
        {
            try
            {
                var token = Encoding.UTF8.GetString(
                    Convert.FromBase64String(HttpUtility.UrlDecode(command.Token)));

                return await _identityIdentityService.ConfirmEmailAsync(
                        token,
                        command.Email,
                        cancellationToken)
                   .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return IdentityResult.Failure(new[] { e.Message });
            }
        }
    }

    #endregion
}
