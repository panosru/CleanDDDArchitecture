namespace Aviant.DDD.Application.Command
{
    using MediatR;

    public interface IHandler<in TCommand, TResponse> :
        IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
    }

    public interface IHandler<in TRequest> :
        IRequestHandler<TRequest, Unit> where TRequest : ICommand<Unit>
    {
    }
}