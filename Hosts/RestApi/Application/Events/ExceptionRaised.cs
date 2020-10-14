namespace CleanDDDArchitecture.Hosts.RestApi.Application.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.ApplicationEvents;

    internal sealed class ExceptionRaised : ApplicationEvent
    {
        public readonly string AssemblyToBlame;

        /// <summary>
        /// </summary>
        // ReSharper disable once MemberCanBeInternal
        public readonly string ErrorMessage;

        public readonly string StackTrace;

        public readonly string SerializedException;

        public ExceptionRaised(
            string   assemblyToBlame,
            string   errorMessage,
            string   stackTrace,
            string   serializedException)
        {
            AssemblyToBlame     = assemblyToBlame;
            ErrorMessage        = errorMessage;
            StackTrace          = stackTrace;
            SerializedException = serializedException;
        }
    }

    internal sealed class ExceptionRaisedHandler : ApplicationEventHandler<ExceptionRaised>
    {
        public override Task Handle(ExceptionRaised request, CancellationToken cancellationToken)
        {
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%");
            Console.WriteLine(request.ErrorMessage);
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%");

            return Task.CompletedTask;
        }
    }
}