using Aviant.Infrastructure.DDD.CrossCutting;
using Microsoft.Extensions.Configuration;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure;

public sealed class TodoItemDomainConfiguration : DomainConfigurationContainer
{
    /// <inheritdoc />
    public TodoItemDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
