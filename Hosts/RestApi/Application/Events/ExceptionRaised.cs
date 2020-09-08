namespace CleanDDDArchitecture.Hosts.RestApi.Application.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class ExceptionRaised : IRequest //TODO: Move to Application exceptions event?
    {
        public readonly string Error;

        public ExceptionRaised(string error) => Error = error;
    }

    public class ExceptionRaisedHandler : IRequestHandler<ExceptionRaised>
    {
    #region IRequestHandler<ExceptionRaised> Members

        public Task<Unit> Handle(ExceptionRaised request, CancellationToken cancellationToken)
        {
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%");
            Console.WriteLine(request.Error);
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%");

            return Task.FromResult(Unit.Value);
        }

    #endregion
    }
}