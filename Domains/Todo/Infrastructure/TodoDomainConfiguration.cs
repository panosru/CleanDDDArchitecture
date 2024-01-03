using Aviant.Infrastructure.DDD.CrossCutting;
using Microsoft.Extensions.Configuration;

namespace CleanDDDArchitecture.Domains.Todo.Infrastructure;

public sealed class TodoDomainConfiguration : DomainConfigurationContainer
{
    /// <inheritdoc />
    public TodoDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
