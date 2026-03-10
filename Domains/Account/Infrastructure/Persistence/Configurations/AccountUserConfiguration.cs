using CleanDDDArchitecture.Domains.Account.Application.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Configurations;

internal sealed class AccountUserConfiguration : IEntityTypeConfiguration<AccountUser>
{
    public void Configure(EntityTypeBuilder<AccountUser> builder)
    {
        builder.Property(user => user.Status)
            .HasConversion<int>()
            .HasDefaultValue(AccountStatus.Active);

        builder.Property(user => user.StatusReason)
            .HasMaxLength(512);
    }
}
