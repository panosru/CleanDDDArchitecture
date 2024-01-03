using Aviant.Infrastructure.DDD.CrossCutting;
using Microsoft.Extensions.Configuration;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure;

public sealed class TodoListDomainConfiguration : DomainConfigurationContainer
{
    /// <inheritdoc />
    public TodoListDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
