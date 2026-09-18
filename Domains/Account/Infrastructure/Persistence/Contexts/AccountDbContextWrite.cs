using Aviant.Infrastructure.Identity.Persistence.Contexts;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Persistence;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;
using CleanDDDArchitecture.Domains.Shared.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;

public sealed class AccountDbContextWrite
    : AuthorizationDbContextWrite<AccountDbContextWrite, AccountUser, AccountRole>, IAccountDbContextWrite
{
    public AccountDbContextWrite(
        DbContextOptions<AccountDbContextWrite> options)
        : base(options)
    { }

    public DbSet<AccountRefreshSession> RefreshSessions => Set<AccountRefreshSession>();

    public DbSet<AccountSecurityEvent> SecurityEvents => Set<AccountSecurityEvent>();

    public DbSet<AccountPasswordHistory> PasswordHistory => Set<AccountPasswordHistory>();

    public DbSet<AccountPhoneVerification> PhoneVerifications => Set<AccountPhoneVerification>();

    public DbSet<AccountTrustedDevice> TrustedDevices => Set<AccountTrustedDevice>();

    /// <summary>Integration events raised by account changes, written in the same transaction.</summary>
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyOutbox();
    }
}
