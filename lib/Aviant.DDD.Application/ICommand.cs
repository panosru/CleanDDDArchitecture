using MediatR;

namespace Aviant.DDD.Application
{
    public interface ICommand<out TResponse> : IRequest<TResponse> {}

    public interface ICommand : ICommand<Unit> {}
}