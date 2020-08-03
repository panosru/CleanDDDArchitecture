namespace Aviant.DDD.Application.Command
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public abstract class Handler<TCommand, TResponse> :
        IHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
    }

    public abstract class Handler<TCommand> :
        IHandler<TCommand, Unit> where TCommand : ICommand<Unit>
    {
        public abstract Task<Unit> Handle(TCommand request, CancellationToken cancellationToken);
    }
}