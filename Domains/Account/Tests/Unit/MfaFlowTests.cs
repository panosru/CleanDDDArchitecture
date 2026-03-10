using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaDisable;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaRecoveryCodes;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaSetup;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaVerify;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class MfaFlowTests
{
    [Fact]
    public async Task MfaSetupCommandHandler_ShouldUseCurrentUser()
    {
        var identityService = new CapturingIdentityService();
        var currentUserId = Guid.Parse("d7fc5e10-36a3-4e33-a2fb-11c61f8c2035");
        var handler = new MfaSetupCommand.MfaSetupCommandHandler(
            identityService,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(new MfaSetupCommand(), CancellationToken.None);

        identityService.SetupUserId.Should().Be(currentUserId);
        result.Should().NotBeNull();
        result!.SharedKey.Should().Be("ABCD EFGH");
    }

    [Fact]
    public async Task MfaVerifyCommandHandler_ShouldEnableMfaForCurrentUser()
    {
        var identityService = new CapturingIdentityService();
        var currentUserId = Guid.Parse("0e8af646-005e-470d-a669-8cf9494f932d");
        var handler = new MfaVerifyCommand.MfaVerifyCommandHandler(
            identityService,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(new MfaVerifyCommand("123456"), CancellationToken.None);

        identityService.VerifyUserId.Should().Be(currentUserId);
        identityService.VerifyCode.Should().Be("123456");
        result.Should().NotBeNull();
        result!.RecoveryCodes.Should().Contain("code-1");
    }

    [Fact]
    public async Task MfaDisableCommandHandler_ShouldDisableMfaForCurrentUser()
    {
        var identityService = new CapturingIdentityService();
        var currentUserId = Guid.Parse("ed8cb7f1-1729-4488-a4d4-78fd15c77ac6");
        var handler = new MfaDisableCommand.MfaDisableCommandHandler(
            identityService,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(new MfaDisableCommand("Abcde1!"), CancellationToken.None);

        identityService.DisableUserId.Should().Be(currentUserId);
        identityService.DisablePassword.Should().Be("Abcde1!");
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task MfaRecoveryCodesCommandHandler_ShouldRegenerateCodesForCurrentUser()
    {
        var identityService = new CapturingIdentityService();
        var currentUserId = Guid.Parse("ec97f105-eabc-4d1f-9bab-9bfe88215456");
        var handler = new MfaRecoveryCodesCommand.MfaRecoveryCodesCommandHandler(
            identityService,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(new MfaRecoveryCodesCommand("Abcde1!"), CancellationToken.None);

        identityService.RecoveryCodesUserId.Should().Be(currentUserId);
        identityService.RecoveryCodesPassword.Should().Be("Abcde1!");
        result.Should().NotBeNull();
        result!.RecoveryCodes.Should().Contain("new-code-1");
    }

    private sealed class CapturingIdentityService : IIdentityService
    {
        public Guid? SetupUserId { get; private set; }

        public Guid? VerifyUserId { get; private set; }

        public string? VerifyCode { get; private set; }

        public Guid? DisableUserId { get; private set; }

        public string? DisablePassword { get; private set; }

        public Guid? RecoveryCodesUserId { get; private set; }

        public string? RecoveryCodesPassword { get; private set; }

        public Task<object?> AuthenticateAsync(
            string username,
            string password,
            string? twoFactorCode = null,
            string? recoveryCode = null,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<MfaSetupTicket?> BeginMfaSetupAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            SetupUserId = userId;

            return Task.FromResult<MfaSetupTicket?>(new MfaSetupTicket("ABCD EFGH", "otpauth://example", false));
        }

        public Task<MfaRecoveryCodesTicket?> EnableMfaAsync(
            Guid userId,
            string code,
            CancellationToken cancellationToken = default)
        {
            VerifyUserId = userId;
            VerifyCode = code;

            return Task.FromResult<MfaRecoveryCodesTicket?>(new MfaRecoveryCodesTicket(["code-1", "code-2"]));
        }

        public Task<IdentityResult> DisableMfaAsync(
            Guid userId,
            string password,
            CancellationToken cancellationToken = default)
        {
            DisableUserId = userId;
            DisablePassword = password;

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<MfaRecoveryCodesTicket?> RegenerateRecoveryCodesAsync(
            Guid userId,
            string password,
            CancellationToken cancellationToken = default)
        {
            RecoveryCodesUserId = userId;
            RecoveryCodesPassword = password;

            return Task.FromResult<MfaRecoveryCodesTicket?>(new MfaRecoveryCodesTicket(["new-code-1", "new-code-2"]));
        }

        public Task<EmailConfirmationTicket?> GenerateEmailConfirmationAsync(
            string email,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<EmailChangeTicket?> GenerateEmailChangeAsync(
            Guid userId,
            string newEmail,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<PasswordResetTicket?> GeneratePasswordResetAsync(
            string email,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> ResetPasswordAsync(
            string email,
            string token,
            string newPassword,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> ChangePasswordAsync(
            Guid userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> ConfirmEmailAsync(
            string token,
            string email,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> ConfirmEmailChangeAsync(
            string currentEmail,
            string newEmail,
            string token,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<string> GetUserNameAsync(
            Guid userId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Guid?> GetUserIdByEmailAsync(
            string email,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<(IdentityResult Result, Guid UserId)> CreateUserAsync(
            string username,
            string password,
            string firstName,
            string lastName,
            IEnumerable<string> roles,
            bool emailConfirmed,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> DeleteUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class StubCurrentUserService : ICurrentUserService
    {
        public StubCurrentUserService(Guid userId) => UserId = userId;

        public Guid UserId { get; }
    }
}
