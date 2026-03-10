using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Configurations;

internal sealed class AccountRefreshSessionConfiguration : IEntityTypeConfiguration<AccountRefreshSession>
{
    public void Configure(EntityTypeBuilder<AccountRefreshSession> builder)
    {
        builder.ToTable("AccountRefreshSessions");

        builder.HasKey(session => session.Id);

        builder.Property(session => session.TokenHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(session => session.CreatedByIp)
            .HasMaxLength(128);

        builder.Property(session => session.UserAgent)
            .HasMaxLength(512);

        builder.Property(session => session.Reason)
            .HasMaxLength(256);

        builder.HasIndex(session => session.UserId);
        builder.HasIndex(session => session.TokenHash).IsUnique();
        builder.HasIndex(session => new { session.UserId, session.RevokedAtUtc });
        builder.HasIndex(session => session.TokenFamilyId);

        builder.HasOne(session => session.User)
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
