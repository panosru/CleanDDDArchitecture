using Aviant.Core.DDD.Domain;

namespace CleanDDDArchitecture.Domains.Todo.Core;

public interface ITodoDomainConfiguration : IDomainConfigurationContainer
{
    const string RouteSegment = "todo";
}
