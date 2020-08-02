namespace CleanArchitecture.Application.Users.Commands.Authenticate
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using IIdentityService = Aviant.DDD.Application.Identity.IService;
    
    public class AuthenticateCommand : Base<object>
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
    }

    public class AuthenticateCommandHandler : Handler<AuthenticateCommand, object>
    {
        private readonly IIdentityService _identityService;

        public AuthenticateCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public override async Task<object> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.Authenticate(request.Username, request.Password);
        }
    }
}