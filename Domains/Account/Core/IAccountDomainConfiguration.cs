using Aviant.Core.DDD.Domain;

namespace CleanDDDArchitecture.Domains.Account.Core;

public interface IAccountDomainConfiguration : IDomainConfigurationContainer
{
    const string RouteSegment = "identity";
}
