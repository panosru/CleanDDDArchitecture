using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSuspend;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnlock;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnsuspend;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Deactivate;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class AccountStateManagementTests
{
    [Fact]
    public async Task DeactivateCommandHandlerShouldUseCurrentUser()
    {
        var service = new CapturingAdministrationService();
        var currentUserId = Guid.Parse("1dcb78ac-440d-4520-a46c-ed6b8139f640");
        var handler = new DeactivateCommand.DeactivateCommandHandler(
            service,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(new DeactivateCommand("Abcde1!"), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        service.DeactivateUserId.Should().Be(currentUserId);
        service.DeactivatePassword.Should().Be("Abcde1!");
    }

    [Fact]
    public async Task AdminSuspendCommandHandlerShouldUseCurrentUserAndTarget()
    {
        var service = new CapturingAdministrationService();
        var currentUserId = Guid.Parse("cf0ff422-a905-40b7-b3ae-ee1f77d2a636");
        var targetUserId = Guid.Parse("0f5611a6-3d85-4624-ab1d-fbb5ca39db43");
        var handler = new AdminSuspendCommand.AdminSuspendCommandHandler(
            service,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(
            new AdminSuspendCommand(targetUserId, "Policy violation"),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        service.SuspendActorUserId.Should().Be(currentUserId);
        service.SuspendTargetUserId.Should().Be(targetUserId);
        service.SuspendReason.Should().Be("Policy violation");
    }

    [Fact]
    public async Task AdminUnsuspendCommandHandlerShouldUseCurrentUserAndTarget()
    {
        var service = new CapturingAdministrationService();
        var currentUserId = Guid.Parse("0900e15c-e6ba-480d-aaac-5907c7f6c635");
        var targetUserId = Guid.Parse("b5977475-6fb2-4d8d-84af-69e4dbcbe001");
        var handler = new AdminUnsuspendCommand.AdminUnsuspendCommandHandler(
            service,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(new AdminUnsuspendCommand(targetUserId), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        service.UnsuspendActorUserId.Should().Be(currentUserId);
        service.UnsuspendTargetUserId.Should().Be(targetUserId);
    }

    [Fact]
    public async Task AdminUnlockCommandHandlerShouldUseCurrentUserAndTarget()
    {
        var service = new CapturingAdministrationService();
        var currentUserId = Guid.Parse("b93f0f39-c996-44c0-b10f-51f9a36a1d99");
        var targetUserId = Guid.Parse("848acb5c-dd77-41ef-a667-f4a0808f7491");
        var handler = new AdminUnlockCommand.AdminUnlockCommandHandler(
            service,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(new AdminUnlockCommand(targetUserId), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        service.UnlockActorUserId.Should().Be(currentUserId);
        service.UnlockTargetUserId.Should().Be(targetUserId);
    }

    private sealed class CapturingAdministrationService : IAccountAdministrationService
    {
        public Guid? DeactivateUserId { get; private set; }

        public string? DeactivatePassword { get; private set; }

        public Guid? SuspendActorUserId { get; private set; }

        public Guid? SuspendTargetUserId { get; private set; }

        public string? SuspendReason { get; private set; }

        public Guid? UnsuspendActorUserId { get; private set; }

        public Guid? UnsuspendTargetUserId { get; private set; }

        public Guid? UnlockActorUserId { get; private set; }

        public Guid? UnlockTargetUserId { get; private set; }

        public Task<IdentityResult> DeactivateAsync(
            Guid userId,
            string currentPassword,
            CancellationToken cancellationToken = default)
        {
            DeactivateUserId = userId;
            DeactivatePassword = currentPassword;

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<IdentityResult> SuspendAsync(
            Guid actorUserId,
            Guid targetUserId,
            string? reason,
            CancellationToken cancellationToken = default)
        {
            SuspendActorUserId = actorUserId;
            SuspendTargetUserId = targetUserId;
            SuspendReason = reason;

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<IdentityResult> UnsuspendAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default)
        {
            UnsuspendActorUserId = actorUserId;
            UnsuspendTargetUserId = targetUserId;

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<IdentityResult> UnlockAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default)
        {
            UnlockActorUserId = actorUserId;
            UnlockTargetUserId = targetUserId;

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<IReadOnlyCollection<string>?> GetRolesAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> ReplaceRolesAsync(
            Guid actorUserId,
            Guid targetUserId,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IReadOnlyCollection<AccountClaimDto>?> GetClaimsAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> ReplaceClaimsAsync(
            Guid actorUserId,
            Guid targetUserId,
            IEnumerable<AccountClaimDto> claims,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IReadOnlyCollection<AccountSecurityEventDto>> GetOwnSecurityEventsAsync(
            Guid userId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IReadOnlyCollection<AccountSecurityEventDto>?> GetSecurityEventsAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class StubCurrentUserService : ICurrentUserService
    {
        public StubCurrentUserService(Guid userId) => UserId = userId;

        public Guid UserId { get; }
    }
}
