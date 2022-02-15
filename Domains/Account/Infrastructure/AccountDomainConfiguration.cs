namespace CleanDDDArchitecture.Domains.Account.Infrastructure;

using Aviant.Infrastructure.CrossCutting;
using Core;
using Microsoft.Extensions.Configuration;

public sealed class AccountDomainConfiguration : DomainConfigurationContainer, IAccountDomainConfiguration
{
    /// <inheritdoc />
    public AccountDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
