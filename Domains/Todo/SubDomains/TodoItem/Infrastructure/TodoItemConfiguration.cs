using Aviant.Infrastructure.DDD.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core;
using Microsoft.Extensions.Configuration;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure;

public sealed class TodoItemDomainConfiguration : DomainConfigurationContainer, ITodoItemDomainConfiguration
{
    /// <inheritdoc />
    public TodoItemDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
