using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitecture.Application.Users.Commands.Authenticate
{
    public class AuthenticateCommand : IRequest<object>
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
    }

    public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, object>
    {
        private readonly IIdentityService _identityService;

        public AuthenticateCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<object> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.Authenticate(request.Username, request.Password);
        }
    }
}