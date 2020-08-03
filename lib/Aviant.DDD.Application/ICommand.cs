namespace Aviant.DDD.Application
{
    using MediatR;

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }

    public interface ICommand : ICommand<Unit>
    {
    }
}