using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetClaims;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetRoles;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSecurityEvents;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateClaims;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateRoles;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class AccountAdministrationQueriesTests
{
    [Fact]
    public async Task AdminGetRolesQueryHandlerShouldUseCurrentUserAndTarget()
    {
        var service = new CapturingAdministrationService();
        var actorUserId = Guid.Parse("02efe77e-e4a4-4a16-8780-f40d2ebe8019");
        var targetUserId = Guid.Parse("315d58e5-7b04-4277-96b3-68ed11374f24");
        var handler = new AdminGetRolesQuery.AdminGetRolesQueryHandler(service, new StubCurrentUserService(actorUserId));

        var result = await handler.Handle(new AdminGetRolesQuery(targetUserId), CancellationToken.None);

        result.Should().ContainInOrder("admin", "user");
        service.GetRolesActorUserId.Should().Be(actorUserId);
        service.GetRolesTargetUserId.Should().Be(targetUserId);
    }

    [Fact]
    public async Task AdminUpdateRolesCommandHandlerShouldUseCurrentUserAndRoles()
    {
        var service = new CapturingAdministrationService();
        var actorUserId = Guid.Parse("d5d056a0-4f4f-4c0c-8544-8d1d931ecb57");
        var targetUserId = Guid.Parse("85994a45-f7ef-49d5-a321-e50df1a4ef6a");
        var handler = new AdminUpdateRolesCommand.AdminUpdateRolesCommandHandler(
            service,
            new StubCurrentUserService(actorUserId));

        var result = await handler.Handle(
            new AdminUpdateRolesCommand(targetUserId, ["admin", "auditor"]),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        service.ReplaceRolesActorUserId.Should().Be(actorUserId);
        service.ReplaceRolesTargetUserId.Should().Be(targetUserId);
        service.ReplaceRoles.Should().Equal("admin", "auditor");
    }

    [Fact]
    public async Task AdminGetClaimsQueryHandlerShouldUseCurrentUserAndTarget()
    {
        var service = new CapturingAdministrationService();
        var actorUserId = Guid.Parse("cbd8a588-812b-4207-af40-ac73337cb8af");
        var targetUserId = Guid.Parse("971cf842-d1c2-4150-88db-a9d4d9186e4c");
        var handler = new AdminGetClaimsQuery.AdminGetClaimsQueryHandler(service, new StubCurrentUserService(actorUserId));

        var result = await handler.Handle(new AdminGetClaimsQuery(targetUserId), CancellationToken.None);

        result.Should().ContainSingle(claim => claim.Type == "permission" && claim.Value == "todo:write");
        service.GetClaimsActorUserId.Should().Be(actorUserId);
        service.GetClaimsTargetUserId.Should().Be(targetUserId);
    }

    [Fact]
    public async Task AdminUpdateClaimsCommandHandlerShouldUseCurrentUserAndClaims()
    {
        var service = new CapturingAdministrationService();
        var actorUserId = Guid.Parse("ec1dc115-e568-4547-9522-d03b581811fd");
        var targetUserId = Guid.Parse("1574c851-bb26-46d6-a0c8-f185e0606c5f");
        var handler = new AdminUpdateClaimsCommand.AdminUpdateClaimsCommandHandler(
            service,
            new StubCurrentUserService(actorUserId));

        var result = await handler.Handle(
            new AdminUpdateClaimsCommand(targetUserId, [new AccountClaimDto("permission", "todo:write")]),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        service.ReplaceClaimsActorUserId.Should().Be(actorUserId);
        service.ReplaceClaimsTargetUserId.Should().Be(targetUserId);
        service.ReplaceClaims.Should().ContainSingle(claim => claim.Type == "permission" && claim.Value == "todo:write");
    }

    [Fact]
    public async Task AdminSecurityEventsQueryHandlerShouldUseCurrentUserAndTarget()
    {
        var service = new CapturingAdministrationService();
        var actorUserId = Guid.Parse("cc1d0c8f-532e-4b19-bf2d-3ba78a5e789e");
        var targetUserId = Guid.Parse("80142933-2718-427f-898d-1c868bc953e5");
        var handler = new AdminSecurityEventsQuery.AdminSecurityEventsQueryHandler(
            service,
            new StubCurrentUserService(actorUserId));

        var result = await handler.Handle(new AdminSecurityEventsQuery(targetUserId), CancellationToken.None);

        result.Should().ContainSingle(item => item.Type == AccountSecurityEventTypes.MfaEnabled);
        service.GetEventsActorUserId.Should().Be(actorUserId);
        service.GetEventsTargetUserId.Should().Be(targetUserId);
    }

    private sealed class CapturingAdministrationService : IAccountAdministrationService
    {
        public Guid? GetRolesActorUserId { get; private set; }

        public Guid? GetRolesTargetUserId { get; private set; }

        public Guid? ReplaceRolesActorUserId { get; private set; }

        public Guid? ReplaceRolesTargetUserId { get; private set; }

        public IReadOnlyCollection<string> ReplaceRoles { get; private set; } = [];

        public Guid? GetClaimsActorUserId { get; private set; }

        public Guid? GetClaimsTargetUserId { get; private set; }

        public Guid? ReplaceClaimsActorUserId { get; private set; }

        public Guid? ReplaceClaimsTargetUserId { get; private set; }

        public IReadOnlyCollection<AccountClaimDto> ReplaceClaims { get; private set; } = [];

        public Guid? GetEventsActorUserId { get; private set; }

        public Guid? GetEventsTargetUserId { get; private set; }

        public Task<IdentityResult> DeactivateAsync(
            Guid userId,
            string currentPassword,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> SuspendAsync(
            Guid actorUserId,
            Guid targetUserId,
            string? reason,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> UnsuspendAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> UnlockAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IReadOnlyCollection<string>?> GetRolesAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default)
        {
            GetRolesActorUserId = actorUserId;
            GetRolesTargetUserId = targetUserId;

            return Task.FromResult<IReadOnlyCollection<string>?>(["admin", "user"]);
        }

        public Task<IdentityResult> ReplaceRolesAsync(
            Guid actorUserId,
            Guid targetUserId,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default)
        {
            ReplaceRolesActorUserId = actorUserId;
            ReplaceRolesTargetUserId = targetUserId;
            ReplaceRoles = roles.ToArray();

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<IReadOnlyCollection<AccountClaimDto>?> GetClaimsAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default)
        {
            GetClaimsActorUserId = actorUserId;
            GetClaimsTargetUserId = targetUserId;

            return Task.FromResult<IReadOnlyCollection<AccountClaimDto>?>([new AccountClaimDto("permission", "todo:write")]);
        }

        public Task<IdentityResult> ReplaceClaimsAsync(
            Guid actorUserId,
            Guid targetUserId,
            IEnumerable<AccountClaimDto> claims,
            CancellationToken cancellationToken = default)
        {
            ReplaceClaimsActorUserId = actorUserId;
            ReplaceClaimsTargetUserId = targetUserId;
            ReplaceClaims = claims.ToArray();

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<IReadOnlyCollection<AccountSecurityEventDto>> GetOwnSecurityEventsAsync(
            Guid userId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IReadOnlyCollection<AccountSecurityEventDto>?> GetSecurityEventsAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default)
        {
            GetEventsActorUserId = actorUserId;
            GetEventsTargetUserId = targetUserId;

            return Task.FromResult<IReadOnlyCollection<AccountSecurityEventDto>?>(
            [
                new AccountSecurityEventDto
                {
                    Id = Guid.NewGuid(),
                    UserId = targetUserId,
                    ActorUserId = actorUserId,
                    Type = AccountSecurityEventTypes.MfaEnabled,
                    Description = "Authenticator app MFA was enabled.",
                    OccurredAtUtc = DateTimeOffset.UtcNow
                }
            ]);
        }
    }

    private sealed class StubCurrentUserService : ICurrentUserService
    {
        public StubCurrentUserService(Guid userId) => UserId = userId;

        public Guid UserId { get; }
    }
}
