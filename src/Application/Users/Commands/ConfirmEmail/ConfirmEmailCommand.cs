namespace CleanArchitecture.Application.Users.Commands.ConfirmEmail
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Aviant.DDD.Application;
    using Aviant.DDD.Application.Command;
    using IIdentityService = Aviant.DDD.Application.Identity.IService;

    public class ConfirmEmailCommand : Base<Result>
    {
        public string Token { get; set; }

        public string Email { get; set; }
    }

    public class ConfirmEmailCommandHandler : Handler<ConfirmEmailCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public ConfirmEmailCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public override async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var token = Encoding.UTF8.GetString(
                Convert.FromBase64String(HttpUtility.UrlDecode(request.Token)));

            return await _identityService.ConfirmEmail(
                token, request.Email);
        }
    }
}