namespace CleanDDDArchitecture.Domains.Account.Infrastructure;

using Aviant.Infrastructure.DDD.CrossCutting;
using Core;
using Microsoft.Extensions.Configuration;

public sealed class AccountDomainConfiguration : DomainConfigurationContainer, IAccountDomainConfiguration
{
    /// <inheritdoc />
    public AccountDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
