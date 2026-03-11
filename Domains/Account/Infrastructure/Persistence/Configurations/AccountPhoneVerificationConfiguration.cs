using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Configurations;

internal sealed class AccountPhoneVerificationConfiguration : IEntityTypeConfiguration<AccountPhoneVerification>
{
    public void Configure(EntityTypeBuilder<AccountPhoneVerification> builder)
    {
        builder.ToTable("AccountPhoneVerifications");

        builder.Property(item => item.PhoneNumber)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(item => item.CodeHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(item => item.Purpose)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(item => item.CreatedByIp)
            .HasMaxLength(128);

        builder.HasIndex(item => new { item.UserId, item.PhoneNumber, item.Purpose, item.ConsumedAtUtc });

        builder.HasOne(item => item.User)
            .WithMany()
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
