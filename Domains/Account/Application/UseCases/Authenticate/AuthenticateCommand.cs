namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate
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

    public class AuthenticateCommandHandler : CommandHandler<AuthenticateCommand, object>
    {
        private readonly IIdentityService _identityIdentityService;

        public AuthenticateCommandHandler(IIdentityService identityIdentityService) =>
            _identityIdentityService = identityIdentityService;

        public override async Task<object> Handle(AuthenticateCommand command, CancellationToken cancellationToken) =>
            await _identityIdentityService.Authenticate(command.Username, command.Password)
               .ConfigureAwait(false);
    }
}