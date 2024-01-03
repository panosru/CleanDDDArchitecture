using Aviant.Infrastructure.DDD.CrossCutting;
using CleanDDDArchitecture.Domains.Account.Core;
using Microsoft.Extensions.Configuration;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure;

public sealed class AccountDomainConfiguration : DomainConfigurationContainer, IAccountDomainConfiguration
{
    /// <inheritdoc />
    public AccountDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
