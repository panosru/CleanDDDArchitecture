namespace Aviant.DDD.Application.Exception
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Exceptions;
    using FluentValidation.Results;

    public class ValidationException : ExceptionBase
    {
        public ValidationException() : 
            base("One or more validation failures have occurred.")
        {
            Failures = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : 
            this()
        {
            var failureGroups = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();

                Failures.Add(propertyName, propertyFailures);
            }
        }

        public IDictionary<string, string[]> Failures { get; }
    }
}