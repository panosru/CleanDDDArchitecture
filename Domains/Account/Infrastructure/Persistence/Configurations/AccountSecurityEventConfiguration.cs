using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Configurations;

internal sealed class AccountSecurityEventConfiguration : IEntityTypeConfiguration<AccountSecurityEvent>
{
    public void Configure(EntityTypeBuilder<AccountSecurityEvent> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Type)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(item => item.Description)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(item => item.IpAddress)
            .HasMaxLength(128);

        builder.Property(item => item.UserAgent)
            .HasMaxLength(512);

        builder.HasIndex(item => new { item.UserId, item.OccurredAtUtc });
    }
}
