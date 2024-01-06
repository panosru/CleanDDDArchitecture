using Aviant.Core.DDD.Domain;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core;

public interface ITodoItemDomainConfiguration : IDomainConfigurationContainer
{
    const string RouteSegment = "item";
}
