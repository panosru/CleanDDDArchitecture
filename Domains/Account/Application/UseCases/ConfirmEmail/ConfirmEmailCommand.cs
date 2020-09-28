namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Identity;

    internal sealed class ConfirmEmailCommand : Command<IdentityResult>
    {
        public ConfirmEmailCommand(string token, string email)
        {
            Token = token;
            Email = email;
        }

        public string Token { get; }

        public string Email { get; }
    }

    internal sealed class ConfirmEmailCommandCommandCommandHandler
        : CommandHandler<ConfirmEmailCommand, IdentityResult>
    {
        private readonly IIdentityService _identityIdentityService;

        public ConfirmEmailCommandCommandCommandHandler(IIdentityService identityIdentityService) =>
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
}