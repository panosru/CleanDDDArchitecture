using Aviant.Application.Identity;
using Aviant.Application.Jobs;
using Aviant.Core.EventSourcing.Services;
using Aviant.Core.Messages;
using Aviant.Core.Services;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmail.Events;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailConfirm;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest.Events;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation.Events;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails;
using FluentAssertions;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class EmailLifecycleTests
{
    [Fact]
    public async Task ConfirmationEmailRequestedApplicationEventHandler_ShouldQueueConfirmationEmail()
    {
        var jobRunner = new CapturingJobRunner();
        var handler = new ConfirmationEmailRequestedApplicationEventHandler(jobRunner);

        await handler.Handle(
            new ConfirmationEmailRequestedApplicationEvent(
                "user@example.com",
                "Test User",
                "encoded-token"),
            CancellationToken.None);

        jobRunner.Email.Should().Be("user@example.com");
        jobRunner.FullName.Should().Be("Test User");
        jobRunner.Token.Should().Be("encoded-token");
    }

    [Fact]
    public async Task EmailChangeRequestedApplicationEventHandler_ShouldQueueChangeEmailConfirmation()
    {
        var jobRunner = new CapturingJobRunner();
        var handler = new EmailChangeRequestedApplicationEventHandler(jobRunner);

        await handler.Handle(
            new EmailChangeRequestedApplicationEvent(
                "current@example.com",
                "new@example.com",
                "Test User",
                "encoded-token"),
            CancellationToken.None);

        jobRunner.CurrentEmail.Should().Be("current@example.com");
        jobRunner.NewEmail.Should().Be("new@example.com");
        jobRunner.FullName.Should().Be("Test User");
        jobRunner.Token.Should().Be("encoded-token");
    }

    [Fact]
    public async Task ChangeEmailConfirmHandler_ShouldPassThroughProtectedToken_AndUpdateAggregateIdentity()
    {
        var identityService = new CapturingIdentityService();
        var aggregateId = Guid.Parse("a4ea848c-f45f-4b5d-b6f3-62215ab59c57");
        var aggregate = AccountAggregate.Create(
            aggregateId,
            "user@example.com",
            "Test",
            "User",
            "user@example.com",
            ["member"],
            true);
        InitialiseServiceLocator(new StubEventsService(aggregate));
        var handler = new ChangeEmailConfirmCommand.ChangeEmailConfirmCommandHandler(identityService, new StubMessages());
        const string protectedToken = "protected-email-change-token";

        var result = await handler.Handle(
            new ChangeEmailConfirmCommand("user@example.com", "new@example.com", protectedToken),
            CancellationToken.None);

        identityService.CurrentEmail.Should().Be("user@example.com");
        identityService.NewEmail.Should().Be("new@example.com");
        identityService.Token.Should().Be(protectedToken);
        result.Email.Should().Be("new@example.com");
        result.UserName.Should().Be("new@example.com");
        result.Events.Should().Contain(e => e is AccountEmailChangedDomainEvent);
    }

    [Fact]
    public async Task UpdateAccountHandler_ShouldRejectDirectEmailChanges()
    {
        var aggregateId = Guid.Parse("a4ea848c-f45f-4b5d-b6f3-62215ab59c57");
        var aggregate = AccountAggregate.Create(
            aggregateId,
            "user@example.com",
            "Test",
            "User",
            "user@example.com",
            ["member"],
            true);
        InitialiseServiceLocator(new StubEventsService(aggregate));
        var messages = new StubMessages();
        var handler = new UpdateAccountCommand.UpdateAccountHandler(messages);

        var result = await handler.Handle(
            new UpdateAccountCommand(
                new AccountAggregateId(aggregateId),
                "Updated",
                "User",
                "new@example.com"),
            CancellationToken.None);

        result.Should().BeNull();
        messages.GetAll().Should().ContainSingle("Use the change-email flow to update the email address.");
    }

    private sealed class CapturingIdentityService : IIdentityService
    {
        public string? CurrentEmail { get; private set; }

        public string? NewEmail { get; private set; }

        public string? Token { get; private set; }

        public Task<object?> AuthenticateAsync(
            string username,
            string password,
            string? twoFactorCode = null,
            string? recoveryCode = null,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<MfaSetupTicket?> BeginMfaSetupAsync(
            Guid userId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<MfaRecoveryCodesTicket?> EnableMfaAsync(
            Guid userId,
            string code,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IdentityResult> DisableMfaAsync(
            Guid userId,
            string password,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<MfaRecoveryCodesTicket?> RegenerateRecoveryCodesAsync(
            Guid userId,
            string password,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<EmailConfirmationTicket?> GenerateEmailConfirmationAsync(string email, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<EmailChangeTicket?> GenerateEmailChangeAsync(Guid userId, string newEmail, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PasswordResetTicket?> GeneratePasswordResetAsync(string email, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IdentityResult> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IdentityResult> ConfirmEmailAsync(string token, string email, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IdentityResult> ConfirmEmailChangeAsync(
            string currentEmail,
            string newEmail,
            string token,
            CancellationToken cancellationToken = default)
        {
            CurrentEmail = currentEmail;
            NewEmail = newEmail;
            Token = token;

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<string> GetUserNameAsync(Guid userId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Guid?> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult<Guid?>(Guid.Parse("a4ea848c-f45f-4b5d-b6f3-62215ab59c57"));

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

        public string? CurrentEmail { get; private set; }

        public string? NewEmail { get; private set; }

        public string? FullName { get; private set; }

        public string? Token { get; private set; }

        public string Run<TJob, TJobOptions>(Action<TJobOptions>? configureJobOptions = null)
            where TJobOptions : class, IJobOptions
            where TJob : IJob<TJobOptions>
        {
            var options = Activator.CreateInstance<TJobOptions>();
            configureJobOptions?.Invoke(options);

            switch (options)
            {
                case SendConfirmationEmailJobOptions confirmationOptions:
                    Email = confirmationOptions.Email;
                    FullName = confirmationOptions.FullName;
                    Token = confirmationOptions.Token;
                    break;
                case SendChangeEmailConfirmationJobOptions changeEmailOptions:
                    CurrentEmail = changeEmailOptions.CurrentEmail;
                    NewEmail = changeEmailOptions.NewEmail;
                    FullName = changeEmailOptions.FullName;
                    Token = changeEmailOptions.Token;
                    break;
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

    private sealed class StubEventsService : IEventsService<AccountAggregate, AccountAggregateId>
    {
        private readonly AccountAggregate _aggregate;

        public StubEventsService(AccountAggregate aggregate) => _aggregate = aggregate;

        public Task PersistAsync(AccountAggregate aggregate, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<AccountAggregate?> RehydrateAsync(AccountAggregateId key, CancellationToken cancellationToken = default) =>
            Task.FromResult<AccountAggregate?>(_aggregate);
    }

    private sealed class StubMessages : IMessages
    {
        private readonly List<string> _messages = [];

        public void AddMessage(string message) => _messages.Add(message);

        public bool HasMessages() => _messages.Count > 0;

        public List<string> GetAll() => _messages;

        public void CleanMessages() => _messages.Clear();
    }

    private sealed class StubServiceContainer : IServiceContainer
    {
        private readonly IServiceProvider _serviceProvider;

        public StubServiceContainer(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public object GetRequiredService(Type type) =>
            _serviceProvider.GetRequiredService(type);

        public T GetRequiredService<T>(Type type) =>
            (T)_serviceProvider.GetRequiredService(type);

        public object GetService(Type type) =>
            _serviceProvider.GetService(type)!;

        public T GetService<T>(Type type) =>
            (T)_serviceProvider.GetService(type)!;
    }

    private static void InitialiseServiceLocator(IEventsService<AccountAggregate, AccountAggregateId> eventsService)
    {
        var services = new ServiceCollection();
        services.AddSingleton(eventsService);
        services.AddSingleton<IServiceContainer, StubServiceContainer>();
        var serviceProvider = services.BuildServiceProvider();
        ServiceLocator.Initialise(serviceProvider);
    }
}
