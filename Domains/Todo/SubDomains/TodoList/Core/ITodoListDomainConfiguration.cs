using Aviant.Core.DDD.Domain;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core;

public interface ITodoListDomainConfiguration : IDomainConfigurationContainer
{
    const string RouteSegment = "list";
}
