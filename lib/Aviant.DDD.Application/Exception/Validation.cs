using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Aviant.DDD.Application.Exception
{
    public class Validation : Aviant.DDD.Domain.Exception.Base
    {
        public IDictionary<string, string[]> Failures { get; }
        
        public Validation() : base("One or more validation failures have occurred.")
        {
            Failures = new Dictionary<string, string[]>();
        }

        public Validation(IEnumerable<ValidationFailure> failures) : this()
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
    }
}