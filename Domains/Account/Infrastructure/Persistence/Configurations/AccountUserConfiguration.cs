using CleanDDDArchitecture.Domains.Account.Application.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Configurations;

internal sealed class AccountUserConfiguration : IEntityTypeConfiguration<AccountUser>
{
    public void Configure(EntityTypeBuilder<AccountUser> builder)
    {
        // Identity config is applied by the base context. This exists so EF has a
        // concrete configuration type in the account infrastructure assembly.
    }
}
