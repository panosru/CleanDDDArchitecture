using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Configurations;

internal sealed class AccountTrustedDeviceConfiguration : IEntityTypeConfiguration<AccountTrustedDevice>
{
    public void Configure(EntityTypeBuilder<AccountTrustedDevice> builder)
    {
        builder.ToTable("AccountTrustedDevices");

        builder.Property(item => item.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(item => item.DeviceName)
            .HasMaxLength(256);

        builder.Property(item => item.CreatedByIp)
            .HasMaxLength(128);

        builder.Property(item => item.UserAgent)
            .HasMaxLength(512);

        builder.HasIndex(item => item.TokenHash)
            .IsUnique();

        builder.HasIndex(item => new { item.UserId, item.RevokedAtUtc, item.ExpiresAtUtc });

        builder.HasOne(item => item.User)
            .WithMany()
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
