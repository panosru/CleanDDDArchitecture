using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminForcePasswordReset;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminListAccounts;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminResendConfirmation;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminRevokeAllSessions;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class AdminAccountOperationsTests
{
    [Fact]
    public async Task AdminListAccountsQueryHandlerShouldUseFiltersAndCurrentUser()
    {
        var service = new CapturingAdministrationService();
        var actorUserId = Guid.Parse("60f65c3c-b59a-4ed6-b9d8-0bca6d37fc4c");
        var handler = new AdminListAccountsQuery.AdminListAccountsQueryHandler(service, new StubCurrentUserService(actorUserId));

        var result = await handler.Handle(new AdminListAccountsQuery("john", "Active", "admin"), CancellationToken.None);

        result.Should().ContainSingle();
        service.SearchActorUserId.Should().Be(actorUserId);
        service.SearchQuery.Should().Be("john");
        service.SearchStatus.Should().Be("Active");
        service.SearchRole.Should().Be("admin");
    }

    [Fact]
    public async Task AdminResendConfirmationCommandHandlerShouldUseCurrentUserAndTarget()
    {
        var service = new CapturingAdministrationService();
        var actorUserId = Guid.Parse("973a5e72-11cc-4fac-8816-8969cf505420");
        var targetUserId = Guid.Parse("cbf54d0f-549a-49ca-b849-8589efb22760");
        var handler = new AdminResendConfirmationCommand.AdminResendConfirmationCommandHandler(
            service,
            new StubCurrentUserService(actorUserId));

        var ticket = await handler.Handle(new AdminResendConfirmationCommand(targetUserId), CancellationToken.None);

        ticket.Should().NotBeNull();
        service.ResendActorUserId.Should().Be(actorUserId);
        service.ResendTargetUserId.Should().Be(targetUserId);
    }

    [Fact]
    public async Task AdminForcePasswordResetCommandHandlerShouldUseCurrentUserAndTarget()
    {
        var service = new CapturingAdministrationService();
        var actorUserId = Guid.Parse("d8c16646-2cf0-4a1f-b89f-d058f6e132bb");
        var targetUserId = Guid.Parse("3035e3af-d86b-406e-9f65-8e22ac338547");
        var handler = new AdminForcePasswordResetCommand.AdminForcePasswordResetCommandHandler(
            service,
            new StubCurrentUserService(actorUserId));

        var ticket = await handler.Handle(new AdminForcePasswordResetCommand(targetUserId), CancellationToken.None);

        ticket.Should().NotBeNull();
        service.ResetActorUserId.Should().Be(actorUserId);
        service.ResetTargetUserId.Should().Be(targetUserId);
    }

    [Fact]
    public async Task AdminRevokeAllSessionsCommandHandlerShouldUseCurrentUserAndTarget()
    {
        var service = new CapturingAdministrationService();
        var actorUserId = Guid.Parse("2f5f98f5-c5ea-4ec4-a76d-fd2fef0a8061");
        var targetUserId = Guid.Parse("8b4f86b2-6453-4cfd-bb55-f7f9d1e859f7");
        var handler = new AdminRevokeAllSessionsCommand.AdminRevokeAllSessionsCommandHandler(
            service,
            new StubCurrentUserService(actorUserId));

        var revoked = await handler.Handle(new AdminRevokeAllSessionsCommand(targetUserId), CancellationToken.None);

        revoked.Should().Be(3);
        service.RevokeActorUserId.Should().Be(actorUserId);
        service.RevokeTargetUserId.Should().Be(targetUserId);
    }

    private sealed class CapturingAdministrationService : IAccountAdministrationService
    {
        public Guid? SearchActorUserId { get; private set; }
        public string? SearchQuery { get; private set; }
        public string? SearchStatus { get; private set; }
        public string? SearchRole { get; private set; }
        public Guid? ResendActorUserId { get; private set; }
        public Guid? ResendTargetUserId { get; private set; }
        public Guid? ResetActorUserId { get; private set; }
        public Guid? ResetTargetUserId { get; private set; }
        public Guid? RevokeActorUserId { get; private set; }
        public Guid? RevokeTargetUserId { get; private set; }

        public Task<IdentityResult> DeactivateAsync(Guid userId, string currentPassword, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IdentityResult> ReactivateAsync(Guid actorUserId, Guid targetUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IdentityResult> SuspendAsync(Guid actorUserId, Guid targetUserId, string? reason, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IdentityResult> UnsuspendAsync(Guid actorUserId, Guid targetUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IdentityResult> UnlockAsync(Guid actorUserId, Guid targetUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<string>?> GetRolesAsync(Guid actorUserId, Guid targetUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IdentityResult> ReplaceRolesAsync(Guid actorUserId, Guid targetUserId, IEnumerable<string> roles, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<AccountClaimDto>?> GetClaimsAsync(Guid actorUserId, Guid targetUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IdentityResult> ReplaceClaimsAsync(Guid actorUserId, Guid targetUserId, IEnumerable<AccountClaimDto> claims, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<AccountSecurityEventDto>> GetOwnSecurityEventsAsync(Guid userId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<AccountSecurityEventDto>?> GetSecurityEventsAsync(Guid actorUserId, Guid targetUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IReadOnlyCollection<AccountAdminSummaryDto>?> SearchAccountsAsync(
            Guid actorUserId,
            string? query,
            string? status,
            string? role,
            CancellationToken cancellationToken = default)
        {
            SearchActorUserId = actorUserId;
            SearchQuery = query;
            SearchStatus = status;
            SearchRole = role;

            return Task.FromResult<IReadOnlyCollection<AccountAdminSummaryDto>?>(
            [
                new AccountAdminSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Username = "john@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    EmailConfirmed = true,
                    Status = "Active",
                    Roles = ["admin"]
                }
            ]);
        }

        public Task<EmailConfirmationTicket?> GenerateEmailConfirmationForUserAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default)
        {
            ResendActorUserId = actorUserId;
            ResendTargetUserId = targetUserId;

            return Task.FromResult<EmailConfirmationTicket?>(new EmailConfirmationTicket("john@example.com", "John Doe", "token"));
        }

        public Task<PasswordResetTicket?> GeneratePasswordResetForUserAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default)
        {
            ResetActorUserId = actorUserId;
            ResetTargetUserId = targetUserId;

            return Task.FromResult<PasswordResetTicket?>(new PasswordResetTicket(targetUserId, "john@example.com", "John Doe", "token"));
        }

        public Task<int?> RevokeAllSessionsAsync(
            Guid actorUserId,
            Guid targetUserId,
            CancellationToken cancellationToken = default)
        {
            RevokeActorUserId = actorUserId;
            RevokeTargetUserId = targetUserId;

            return Task.FromResult<int?>(3);
        }

        public Task<AccountDeletionRequestResult> RequestAccountDeletionAsync(
            Guid userId,
            string currentPassword,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> ConfirmAccountDeletionAsync(
            string email,
            string token,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> RequestPhoneVerificationAsync(
            Guid userId,
            string phoneNumber,
            bool isChange,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> VerifyPhoneVerificationAsync(
            Guid userId,
            string phoneNumber,
            string code,
            bool isChange,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class StubCurrentUserService : ICurrentUserService
    {
        public StubCurrentUserService(Guid userId) => UserId = userId;

        public Guid UserId { get; }
    }
}
