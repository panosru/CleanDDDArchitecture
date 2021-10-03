namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate
{
    using System.Security.Authentication;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Identity;

    internal sealed class AuthenticateCommand : Command<object>
    {
        public AuthenticateCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }

        private string Username { get; }

        private string Password { get; }

        #region Nested type: AuthenticateCommandHandler

        internal sealed class AuthenticateCommandHandler : CommandHandler<AuthenticateCommand, object>
        {
            private readonly IIdentityService _identityIdentityService;

            public AuthenticateCommandHandler(IIdentityService identityIdentityService) =>
                _identityIdentityService = identityIdentityService;

            public override async Task<object> Handle(AuthenticateCommand command, CancellationToken cancellationToken)
            {
                var user = await _identityIdentityService
                   .AuthenticateAsync(command.Username, command.Password, cancellationToken)
                   .ConfigureAwait(false);

                return user ?? throw new AuthenticationException();
            }
        }

        #endregion
    }
}
