using MediatR;

namespace Aviant.DDD.Application.Command
{
    public abstract class Base<TResponse> : ICommand<TResponse> {}
    
    public abstract class Base : Base<Unit>, ICommand {}
}