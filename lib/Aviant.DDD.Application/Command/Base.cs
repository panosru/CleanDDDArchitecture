namespace Aviant.DDD.Application.Command
{
    using MediatR;

    public abstract class Base<TResponse> : ICommand<TResponse>
    {
    }

    public abstract class Base : Base<Unit>, ICommand
    {
    }
}