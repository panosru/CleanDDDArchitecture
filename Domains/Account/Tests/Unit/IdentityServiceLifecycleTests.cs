using System.Net;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Notifications;
using CleanDDDArchitecture.Domains.Account.Infrastructure;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Identity;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class IdentityServiceLifecycleTests
{
    [Fact]
    public async Task ChangePassword_ShouldRejectRecentPasswordReuse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var sut = CreateSut();
        var service = sut.Service;

        var created = await service.CreateUserAsync(
            "history.user@example.local",
            "Abcde1!",
            "History",
            "User",
            ["member"],
            true,
            cancellationToken);

        created.Result.Succeeded.Should().BeTrue();

        var changed = await service.ChangePasswordAsync(created.UserId, "Abcde1!", "Xyzab2!", cancellationToken);
        changed.Succeeded.Should().BeTrue();

        var reused = await service.ChangePasswordAsync(created.UserId, "Xyzab2!", "Abcde1!", cancellationToken);

        reused.Succeeded.Should().BeFalse();
        reused.Errors.Should().ContainSingle(error => error.Contains("last 5 passwords", StringComparison.Ordinal));
    }

    [Fact]
    public async Task RequestAndVerifyPhoneVerification_ShouldConfirmPhoneNumber()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var sut = CreateSut();
        var service = sut.Service;

        var created = await service.CreateUserAsync(
            "phone.user@example.local",
            "Abcde1!",
            "Phone",
            "User",
            ["member"],
            true,
            cancellationToken);

        created.Result.Succeeded.Should().BeTrue();

        var requested = await service.RequestPhoneVerificationAsync(
            created.UserId,
            "+306900000000",
            false,
            cancellationToken);

        requested.Succeeded.Should().BeTrue();
        sut.PhoneSender.PhoneNumber.Should().Be("+306900000000");
        sut.PhoneSender.IsChangeRequest.Should().BeFalse();
        sut.PhoneSender.Code.Should().NotBeNullOrWhiteSpace();

        var verified = await service.VerifyPhoneVerificationAsync(
            created.UserId,
            "+306900000000",
            sut.PhoneSender.Code!,
            false,
            cancellationToken);

        verified.Succeeded.Should().BeTrue();

        var user = await sut.Context.Users.SingleAsync(item => item.Id == created.UserId, cancellationToken);
        user.PhoneNumber.Should().Be("+306900000000");
        user.PhoneNumberConfirmed.Should().BeTrue();
    }

    [Fact]
    public async Task RequestAndConfirmDeletion_ShouldMarkAccountDeleted()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var sut = CreateSut();
        var service = sut.Service;

        var created = await service.CreateUserAsync(
            "delete.user@example.local",
            "Abcde1!",
            "Delete",
            "User",
            ["member"],
            true,
            cancellationToken);

        created.Result.Succeeded.Should().BeTrue();

        var request = await service.RequestAccountDeletionAsync(created.UserId, "Abcde1!", cancellationToken);

        request.Result.Succeeded.Should().BeTrue();
        request.Ticket.Should().NotBeNull();

        var confirmed = await service.ConfirmAccountDeletionAsync(
            request.Ticket!.Email,
            request.Ticket.Token,
            cancellationToken);

        confirmed.Succeeded.Should().BeTrue();

        var user = await sut.Context.Users.SingleAsync(item => item.Id == created.UserId, cancellationToken);
        user.Status.Should().Be(AccountStatus.Deleted);
    }

    [Fact]
    public async Task ReactivateAsync_ShouldRestoreDeletedAccountForAdministrator()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var sut = CreateSut();
        var service = sut.Service;

        var admin = await service.CreateUserAsync(
            "admin.user@example.local",
            "Abcde1!",
            "Admin",
            "User",
            ["admin"],
            true,
            cancellationToken);
        var member = await service.CreateUserAsync(
            "member.user@example.local",
            "Abcde1!",
            "Member",
            "User",
            ["member"],
            true,
            cancellationToken);

        admin.Result.Succeeded.Should().BeTrue();
        member.Result.Succeeded.Should().BeTrue();

        var deletionRequest = await service.RequestAccountDeletionAsync(member.UserId, "Abcde1!", cancellationToken);
        deletionRequest.Result.Succeeded.Should().BeTrue();
        var deletion = await service.ConfirmAccountDeletionAsync(
            deletionRequest.Ticket!.Email,
            deletionRequest.Ticket.Token,
            cancellationToken);
        deletion.Succeeded.Should().BeTrue();

        var reactivated = await service.ReactivateAsync(admin.UserId, member.UserId, cancellationToken);

        reactivated.Succeeded.Should().BeTrue();
        var restoredUser = await sut.Context.Users.SingleAsync(item => item.Id == member.UserId, cancellationToken);
        restoredUser.Status.Should().Be(AccountStatus.Active);
    }

    private static Sut CreateSut()
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
                    ["Jwt:Refresh:ExpirationDurationInMinutes"] = "10080",
                    ["PasswordHistory:RememberCount"] = "5",
                    ["PhoneVerification:ExpirationDurationInMinutes"] = "10",
                    ["PhoneVerification:CodeLength"] = "6",
                    ["TrustedDevices:ExpirationDurationInDays"] = "30"
                })
            .Build();

        var options = new DbContextOptionsBuilder<AccountDbContextWrite>()
            .UseInMemoryDatabase($"account-identity-lifecycle-tests-{Guid.NewGuid():N}")
            .Options;
        var context = new AccountDbContextWrite(options);
        var userManager = CreateUserManager(context);
        var roleManager = CreateRoleManager(context);
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Loopback;
        httpContext.Request.Headers.UserAgent = "IdentityServiceLifecycleTests";
        var phoneSender = new CapturingPhoneVerificationSender();
        var dataProtectionDirectory = Directory.CreateDirectory(
            Path.Combine(Path.GetTempPath(), $"cleanddd-tests-{Guid.NewGuid():N}"));

        return new Sut(
            context,
            phoneSender,
            new IdentityService(
                userManager,
                roleManager,
                new AccountDomainConfiguration(configuration),
                context,
                DataProtectionProvider.Create(dataProtectionDirectory),
                new HttpContextAccessor { HttpContext = httpContext },
                new StubHttpClientFactory(),
                phoneSender),
            dataProtectionDirectory);
    }

    private static UserManager<AccountUser> CreateUserManager(AccountDbContextWrite context)
    {
        var store = new UserStore<AccountUser, AccountRole, AccountDbContextWrite, Guid>(context);
        var options = new IdentityOptions
        {
            Password =
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = true,
                RequiredLength = 6
            }
        };

        return new UserManager<AccountUser>(
            store,
            Options.Create(options),
            new PasswordHasher<AccountUser>(),
            [],
            [],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            NullLogger<UserManager<AccountUser>>.Instance);
    }

    private static RoleManager<AccountRole> CreateRoleManager(AccountDbContextWrite context)
    {
        var roleStore = new RoleStore<AccountRole, AccountDbContextWrite, Guid>(context);

        return new RoleManager<AccountRole>(
            roleStore,
            [],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            NullLogger<RoleManager<AccountRole>>.Instance);
    }

    private sealed class CapturingPhoneVerificationSender : IPhoneVerificationSender
    {
        public string? PhoneNumber { get; private set; }

        public string? Code { get; private set; }

        public bool? IsChangeRequest { get; private set; }

        public Task SendVerificationCodeAsync(
            string phoneNumber,
            string code,
            bool isChangeRequest,
            CancellationToken cancellationToken = default)
        {
            PhoneNumber = phoneNumber;
            Code = code;
            IsChangeRequest = isChangeRequest;

            return Task.CompletedTask;
        }
    }

    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => throw new NotSupportedException();
    }

    private sealed class Sut : IDisposable
    {
        private readonly DirectoryInfo _dataProtectionDirectory;

        public Sut(
            AccountDbContextWrite context,
            CapturingPhoneVerificationSender phoneSender,
            IdentityService service,
            DirectoryInfo dataProtectionDirectory)
        {
            Context = context;
            PhoneSender = phoneSender;
            Service = service;
            _dataProtectionDirectory = dataProtectionDirectory;
        }

        public AccountDbContextWrite Context { get; }

        public CapturingPhoneVerificationSender PhoneSender { get; }

        public IdentityService Service { get; }

        public void Dispose()
        {
            Context.Dispose();

            if (_dataProtectionDirectory.Exists)
                _dataProtectionDirectory.Delete(recursive: true);
        }
    }
}
