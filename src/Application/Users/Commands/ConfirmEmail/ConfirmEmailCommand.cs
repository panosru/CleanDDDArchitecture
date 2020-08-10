namespace CleanDDDArchitecture.Application.Users.Commands.ConfirmEmail
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Aviant.DDD.Application;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Identity;

    public class ConfirmEmailCommand : CommandBase<IdentityResult>
    {
        public string Token { get; set; }

        public string Email { get; set; }
    }

    public class ConfirmEmailCommandCommandHandler : CommandHandler<ConfirmEmailCommand, IdentityResult>
    {
        private readonly IIdentityService _identityIdentityService;

        public ConfirmEmailCommandCommandHandler(IIdentityService identityIdentityService)
        {
            _identityIdentityService = identityIdentityService;
        }

        public override async Task<IdentityResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var token = Encoding.UTF8.GetString(
                    Convert.FromBase64String(HttpUtility.UrlDecode(request.Token)));

                return await _identityIdentityService.ConfirmEmail(
                    token, request.Email);
            }
            catch (Exception e)
            {
                return IdentityResult.Failure(new []{e.Message});
            }
        }
    }
}