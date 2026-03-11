using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Configurations;

internal sealed class AccountPasswordHistoryConfiguration : IEntityTypeConfiguration<AccountPasswordHistory>
{
    public void Configure(EntityTypeBuilder<AccountPasswordHistory> builder)
    {
        builder.ToTable("AccountPasswordHistory");

        builder.Property(item => item.PasswordHash)
            .HasMaxLength(1024)
            .IsRequired();

        builder.HasIndex(item => new { item.UserId, item.CreatedAtUtc });

        builder.HasOne(item => item.User)
            .WithMany()
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
