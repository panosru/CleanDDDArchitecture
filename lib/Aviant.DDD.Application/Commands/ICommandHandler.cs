namespace Aviant.DDD.Application.Commands
{
    using MediatR;

    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
    }

    public interface ICommandHandler<in TRequest> : IRequestHandler<TRequest>
        where TRequest : ICommand<Unit>
    {
    }
}