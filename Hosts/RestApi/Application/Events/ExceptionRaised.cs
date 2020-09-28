namespace CleanDDDArchitecture.Hosts.RestApi.Application.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public sealed class ExceptionRaised : IRequest //TODO: Move to Application exceptions event?
    {
        /// <summary>
        /// </summary>
        // ReSharper disable once MemberCanBeInternal
        public readonly string Error;

        /// <summary>
        /// </summary>
        /// <param name="error"></param>
        internal ExceptionRaised(string error) => Error = error;
    }

    public sealed class ExceptionRaisedHandler : IRequestHandler<ExceptionRaised>
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