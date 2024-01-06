using Aviant.Infrastructure.DDD.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.Core;
using Microsoft.Extensions.Configuration;

namespace CleanDDDArchitecture.Domains.Todo.Infrastructure;

public sealed class TodoDomainConfiguration : DomainConfigurationContainer, ITodoDomainConfiguration
{
    /// <inheritdoc />
    public TodoDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
