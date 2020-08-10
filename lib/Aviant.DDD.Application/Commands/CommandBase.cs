namespace Aviant.DDD.Application.Commands
{
    using MediatR;

    public abstract class CommandBase<TResponse> : ICommand<TResponse>
    {
    }

    public abstract class CommandBase : CommandBase<Unit>, ICommand
    {
    }
}