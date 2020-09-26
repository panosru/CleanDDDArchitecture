namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Identity;

    public class ConfirmEmailCommand : Command<IdentityResult>
    {
        public string Token { get; set; }

        public string Email { get; set; }
    }

    public class ConfirmEmailCommandCommandCommandHandler
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

                return await _identityIdentityService.ConfirmEmail(
                        token,
                        command.Email)
                   .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return IdentityResult.Failure(new[] { e.Message });
            }
        }
    }
}