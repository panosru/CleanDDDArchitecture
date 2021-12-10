namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure;

using Aviant.DDD.Infrastructure.CrossCutting;
using Microsoft.Extensions.Configuration;

public sealed class TodoItemDomainConfiguration : DomainConfigurationContainer
{
    /// <inheritdoc />
    public TodoItemDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}