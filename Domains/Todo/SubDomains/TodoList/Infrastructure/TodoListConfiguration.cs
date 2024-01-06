using Aviant.Infrastructure.DDD.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core;
using Microsoft.Extensions.Configuration;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure;

public sealed class TodoListDomainConfiguration : DomainConfigurationContainer, ITodoListDomainConfiguration
{
    /// <inheritdoc />
    public TodoListDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
