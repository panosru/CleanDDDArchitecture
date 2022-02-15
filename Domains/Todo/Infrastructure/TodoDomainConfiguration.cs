namespace CleanDDDArchitecture.Domains.Todo.Infrastructure;

using Aviant.Infrastructure.DDD.CrossCutting;
using Microsoft.Extensions.Configuration;

public sealed class TodoDomainConfiguration : DomainConfigurationContainer
{
    /// <inheritdoc />
    public TodoDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
