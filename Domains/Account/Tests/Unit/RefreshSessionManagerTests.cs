using System.Net;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Infrastructure;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class RefreshSessionManagerTests
{
    [Fact]
    public async Task IssueRefresh_ShouldRotateSessionAndAllowExplicitLogout()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var (context, manager) = CreateSut();
        var user = new AccountUser
        {
            Id = Guid.NewGuid(),
            UserName = "refresh.user@example.local",
            Email = "refresh.user@example.local",
            NormalizedUserName = "REFRESH.USER@EXAMPLE.LOCAL",
            NormalizedEmail = "REFRESH.USER@EXAMPLE.LOCAL",
            FirstName = "Refresh",
            LastName = "Tester",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        var issued = await manager.IssueTokensAsync(user, cancellationToken);
        issued.RefreshToken.Should().NotBeNullOrWhiteSpace();

        var initialSessions = await manager.GetActiveSessionsAsync(user.Id, cancellationToken);
        initialSessions.Should().ContainSingle();

        var refreshed = await manager.RefreshAsync(issued.RefreshToken, cancellationToken);
        refreshed.Should().NotBeNull();
        refreshed!.RefreshToken.Should().NotBe(issued.RefreshToken);

        var activeSessions = await manager.GetActiveSessionsAsync(user.Id, cancellationToken);
        activeSessions.Should().ContainSingle();
        activeSessions.Single().Id.Should().NotBe(initialSessions.Single().Id);

        var revoked = await manager.RevokeRefreshTokenAsync(refreshed.RefreshToken, cancellationToken);
        revoked.Should().BeTrue();

        var sessionsAfterLogout = await manager.GetActiveSessionsAsync(user.Id, cancellationToken);
        sessionsAfterLogout.Should().BeEmpty();
    }

    private static (AccountDbContextWrite Context, RefreshSessionManager Manager) CreateSut()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:Issuer"] = "CleanDDDArchitecture",
                    ["Jwt:Audience"] = "CleanDDDArchitecture.Clients",
                    ["Jwt:ClockSkewInMinutes"] = "0",
                    ["Jwt:Access:Key256Bit"] = "12345678901234567890123456789012",
                    ["Jwt:Access:Key512Bit"] =
                        "1234567890123456789012345678901212345678901234567890123456789012",
                    ["Jwt:Access:ExpirationDurationInMinutes"] = "15",
                    ["Jwt:Refresh:Key256Bit"] = "abcdefghijklmnopqrstuvwxyz123456",
                    ["Jwt:Refresh:Key512Bit"] =
                        "abcdefghijklmnopqrstuvwxyz123456ABCDEFGHIJKLMNOPQRSTUVWXYZ7890!@",
                    ["Jwt:Refresh:ExpirationDurationInMinutes"] = "10080"
                })
            .Build();

        var options = new DbContextOptionsBuilder<AccountDbContextWrite>()
            .UseInMemoryDatabase($"account-refresh-tests-{Guid.NewGuid():N}")
            .Options;

        var context = new AccountDbContextWrite(options);
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Loopback;
        httpContext.Request.Headers.UserAgent = "UnitTest";

        return (
            context,
            new RefreshSessionManager(
                context,
                new AccountDomainConfiguration(configuration),
                new HttpContextAccessor { HttpContext = httpContext },
                CreateUserManager(context)));
    }

    private static UserManager<AccountUser> CreateUserManager(AccountDbContextWrite context)
    {
        var store = new UserStore<AccountUser, AccountRole, AccountDbContextWrite, Guid>(context);

        return new UserManager<AccountUser>(
            store,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<AccountUser>(),
            [],
            [],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            NullLogger<UserManager<AccountUser>>.Instance);
    }
}
