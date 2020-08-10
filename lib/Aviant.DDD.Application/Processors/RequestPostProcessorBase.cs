namespace Aviant.DDD.Application.Processors
{
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class RequestPostProcessorBase<TRequest, TResponse> :
        IRequestPostProcessor<TRequest, TResponse>
        where TRequest : notnull
    {
        public abstract Task Process(TRequest request, TResponse response, CancellationToken cancellationToken);
    }
}