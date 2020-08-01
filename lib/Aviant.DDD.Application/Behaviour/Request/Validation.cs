using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = Aviant.DDD.Application.Exception.Validation;
using FluentValidation;
using MediatR;

namespace Aviant.DDD.Application.Behaviour.Request
{
    public abstract class Validation<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public Validation(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                
                var failures = validationResults.SelectMany(r => r.Errors)
                    .Where(f => f != null).ToList();
                
                if (0 != failures.Count)
                    throw new ValidationException(failures);
            }

            return await next();
        }
    }
}