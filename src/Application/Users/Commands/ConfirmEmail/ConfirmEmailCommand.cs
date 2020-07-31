using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Users.Commands.ConfirmEmail
{
    public class ConfirmEmailCommand : IRequest<Result>
    {
        public string Token { get; set; }

        public string Email { get; set; }
    }

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public ConfirmEmailCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var token = System.Text.Encoding.UTF8.GetString(
                System.Convert.FromBase64String(request.Token));
            
            return await _identityService.ConfirmEmail(
                token, request.Email);
        }
    }
}