namespace Aviant.DDD.Application.Processors
{
    public interface IRequestPreProcessor<in TRequest> : MediatR.Pipeline.IRequestPreProcessor<TRequest>
        where TRequest : notnull
    {
    }
}