namespace Aviant.DDD.Application.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public abstract class CommandCommandHandler<TCommand, TResponse> :
        ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
    }

    public abstract class CommandCommandHandler<TCommand> :
        ICommandHandler<TCommand> where TCommand : ICommand<Unit>
    {
        public abstract Task<Unit> Handle(TCommand request, CancellationToken cancellationToken);
    }
}