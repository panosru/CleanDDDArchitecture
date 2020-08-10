namespace Aviant.DDD.Application.Commands
{
    using MediatR;

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }

    public interface ICommand : ICommand<Unit>
    {
    }
}