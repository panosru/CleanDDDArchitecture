namespace CleanDDDArchitecture.Application.Users.Commands.Authenticate
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Identity;

    public class AuthenticateCommand : CommandBase<object>
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class AuthenticateCommandCommandCommandCommandHandler : CommandCommandHandler<AuthenticateCommand, object>
    {
        private readonly IIdentityService _identityIdentityService;

        public AuthenticateCommandCommandCommandCommandHandler(IIdentityService identityIdentityService)
        {
            _identityIdentityService = identityIdentityService;
        }

        public override async Task<object> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            return await _identityIdentityService.Authenticate(request.Username, request.Password);
        }
    }
}