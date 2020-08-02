using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace CleanArchitecture.Application.Common.Events
{
    public partial class ExceptionRaised : IRequest
    {
        public readonly string _error;

        public ExceptionRaised(string error)
        {
            _error = error;
        }
    }

    public class ExceptionRaisedHandler : IRequestHandler<ExceptionRaised>
    {
        public Task<Unit> Handle(ExceptionRaised request, CancellationToken cancellationToken)
        {
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%");
            Console.WriteLine(request._error);
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%");
            
            return Task.FromResult(Unit.Value);
        }
    }
}