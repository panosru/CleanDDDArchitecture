using Aviant.Application.Identity;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangePassword;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword.Events;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ResetPassword;
using FluentAssertions;
using Hangfire.States;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class PasswordLifecycleTests
{
    [Fact]
    public async Task PasswordResetRequestedApplicationEventHandler_ShouldQueueResetEmail()
    {
        var jobRunner = new CapturingJobRunner();
        var handler = new PasswordResetRequestedApplicationEventHandler(jobRunner);

        await handler.Handle(
            new PasswordResetRequestedApplicationEvent(
                "user@example.com",
                "Test User",
                "encoded-token"),
            CancellationToken.None);

        jobRunner.Email.Should().Be("user@example.com");
        jobRunner.FullName.Should().Be("Test User");
        jobRunner.Token.Should().Be("encoded-token");
        jobRunner.RunInvocations.Should().Be(1);
    }

    [Fact]
    public async Task ResetPasswordCommandHandler_ShouldReturnIdentityResult()
    {
        var identityService = new CapturingIdentityService();
        var handler = new ResetPasswordCommand.ResetPasswordCommandHandler(identityService);

        var result = await handler.Handle(
            new ResetPasswordCommand("user@example.com", "encoded-token", "Abcde1!"),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        identityService.ResetEmail.Should().Be("user@example.com");
        identityService.ResetToken.Should().Be("encoded-token");
        identityService.ResetPassword.Should().Be("Abcde1!");
    }

    [Fact]
    public async Task ChangePasswordCommandHandler_ShouldUseCurrentUser()
    {
        var identityService = new CapturingIdentityService();
        var currentUserId = Guid.Parse("ed6b090b-2089-4f7c-bb30-e865dbbd6249");
        var handler = new ChangePasswordCommand.ChangePasswordCommandHandler(
            identityService,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(
            new ChangePasswordCommand("Abcde1!", "Xyzab2!"),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        identityService.ChangePasswordUserId.Should().Be(currentUserId);
        identityService.ChangePasswordCurrentPassword.Should().Be("Abcde1!");
        identityService.ChangePasswordNewPassword.Should().Be("Xyzab2!");
    }

    private sealed class CapturingIdentityService : IIdentityService
    {
        public string? ResetEmail { get; private set; }

        public string? ResetToken { get; private set; }

        public string? ResetPassword { get; private set; }

        public Guid? ChangePasswordUserId { get; private set; }

        public string? ChangePasswordCurrentPassword { get; private set; }

        public string? ChangePasswordNewPassword { get; private set; }

        public Task<object?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<EmailConfirmationTicket?> GenerateEmailConfirmationAsync(
            string email,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<EmailChangeTicket?> GenerateEmailChangeAsync(
            Guid userId,
            string newEmail,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<PasswordResetTicket?> GeneratePasswordResetAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult<PasswordResetTicket?>(new PasswordResetTicket(Guid.NewGuid(), email, "Test User", "encoded-token"));

        public Task<IdentityResult> ResetPasswordAsync(
            string email,
            string token,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            ResetEmail = email;
            ResetToken = token;
            ResetPassword = newPassword;

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<IdentityResult> ChangePasswordAsync(
            Guid userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            ChangePasswordUserId = userId;
            ChangePasswordCurrentPassword = currentPassword;
            ChangePasswordNewPassword = newPassword;

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<IdentityResult> ConfirmEmailAsync(string token, string email, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IdentityResult> ConfirmEmailChangeAsync(
            string currentEmail,
            string newEmail,
            string token,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<string> GetUserNameAsync(Guid userId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Guid?> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<(IdentityResult Result, Guid UserId)> CreateUserAsync(
            string username,
            string password,
            string firstName,
            string lastName,
            IEnumerable<string> roles,
            bool emailConfirmed,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class CapturingJobRunner : IJobRunner
    {
        public string? Email { get; private set; }

        public string? FullName { get; private set; }

        public string? Token { get; private set; }

        public int RunInvocations { get; private set; }

        public string Run<TJob, TJobOptions>(Action<TJobOptions>? configureJobOptions = null)
            where TJobOptions : class, IJobOptions
            where TJob : IJob<TJobOptions>
        {
            RunInvocations++;

            var options = Activator.CreateInstance<TJobOptions>();
            configureJobOptions?.Invoke(options);

            if (options is SendPasswordResetEmailJobOptions emailOptions)
            {
                Email = emailOptions.Email;
                FullName = emailOptions.FullName;
                Token = emailOptions.Token;
            }

            return "job-id";
        }

        public string RunInState<TJob, TJobOptions>(IState state, Action<TJobOptions>? configureJobOptions = null)
            where TJobOptions : class, IJobOptions
            where TJob : IJob<TJobOptions> => throw new NotSupportedException();

        public string RunWithDelay<TJob, TJobOptions>(TimeSpan delay, Action<TJobOptions>? configureJobOptions = null)
            where TJobOptions : class, IJobOptions
            where TJob : IJob<TJobOptions> => throw new NotSupportedException();

        public string RunAtDateTime<TJob, TJobOptions>(DateTime dateTime, Action<TJobOptions>? configureJobOptions = null)
            where TJobOptions : class, IJobOptions
            where TJob : IJob<TJobOptions> => throw new NotSupportedException();

        public string RunAfter<TJob, TJobOptions>(string previousJobId, Action<TJobOptions>? configureJobOptions = null)
            where TJobOptions : class, IJobOptions
            where TJob : IJob<TJobOptions> => throw new NotSupportedException();

        public string RunRecurring<TJob, TJobOptions>(string jobId, string cron, Action<TJobOptions>? configureJobOptions = null)
            where TJobOptions : class, IJobOptions
            where TJob : IJob<TJobOptions> => throw new NotSupportedException();

        public void TriggerRecurringJob(string id) => throw new NotSupportedException();

        public void RemoveRecurringJob(string id) => throw new NotSupportedException();
    }

    private sealed class StubCurrentUserService : ICurrentUserService
    {
        public StubCurrentUserService(Guid userId) => UserId = userId;

        public Guid UserId { get; }
    }
}
