namespace CleanDDDArchitecture.Application.Accounts.Commands.Authenticate
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Identity;

    public class AuthenticateCommand : Command<object>
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class AuthenticateCommandCommandCommandHandler : CommandHandler<AuthenticateCommand, object>
    {
        private readonly IIdentityService _identityIdentityService;

        public AuthenticateCommandCommandCommandHandler(IIdentityService identityIdentityService) =>
            _identityIdentityService = identityIdentityService;

        public override async Task<object> Handle(AuthenticateCommand command, CancellationToken cancellationToken) =>
            await _identityIdentityService.Authenticate(command.Username, command.Password);
    }
}