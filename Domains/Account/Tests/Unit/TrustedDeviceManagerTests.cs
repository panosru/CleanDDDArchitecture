using System.Net;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Domains.Account.Infrastructure;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class TrustedDeviceManagerTests
{
    [Fact]
    public async Task IssueAndRevokeTrustedDevice_ShouldManageLifecycle()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var context = CreateContext();
        var userId = Guid.NewGuid();
        context.Users.Add(
            new AccountUser
            {
                Id = userId,
                UserName = "trusted.user@example.local",
                Email = "trusted.user@example.local",
                NormalizedUserName = "TRUSTED.USER@EXAMPLE.LOCAL",
                NormalizedEmail = "TRUSTED.USER@EXAMPLE.LOCAL",
                FirstName = "Trusted",
                LastName = "User",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            });
        await context.SaveChangesAsync(cancellationToken);

        var manager = new TrustedDeviceManager(
            context,
            CreateHttpContextAccessor(),
            new AccountDomainConfiguration(CreateConfiguration()));

        var token = await manager.IssueTrustedDeviceTokenAsync(userId, "MacBook Pro", cancellationToken);

        token.Should().NotBeNullOrWhiteSpace();
        (await manager.IsTrustedDeviceAsync(userId, token, cancellationToken)).Should().BeTrue();

        var devices = await manager.GetTrustedDevicesAsync(userId, cancellationToken);
        devices.Should().ContainSingle();
        var device = devices.Single();
        device.DeviceName.Should().Be("MacBook Pro");
        device.CreatedByIp.Should().Be(IPAddress.Loopback.ToString());

        var revoked = await manager.RevokeTrustedDeviceAsync(userId, device.Id, cancellationToken);

        revoked.Should().BeTrue();
        (await manager.IsTrustedDeviceAsync(userId, token, cancellationToken)).Should().BeFalse();
        (await manager.GetTrustedDevicesAsync(userId, cancellationToken)).Should().BeEmpty();
    }

    private static AccountDbContextWrite CreateContext()
    {
        var options = new DbContextOptionsBuilder<AccountDbContextWrite>()
            .UseInMemoryDatabase($"account-trusted-device-tests-{Guid.NewGuid():N}")
            .Options;

        return new AccountDbContextWrite(options);
    }

    private static IConfiguration CreateConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["TrustedDevices:ExpirationDurationInDays"] = "30"
                })
            .Build();

    private static IHttpContextAccessor CreateHttpContextAccessor()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Loopback;
        httpContext.Request.Headers.UserAgent = "TrustedDeviceManagerTests";

        return new HttpContextAccessor { HttpContext = httpContext };
    }
}
