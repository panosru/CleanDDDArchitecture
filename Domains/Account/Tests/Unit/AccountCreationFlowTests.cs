using System.Text;
using Aviant.Application.Identity;
using Aviant.Application.Jobs;
using Aviant.Core.Messages;
using Aviant.Core.EventSourcing.Services;
using Aviant.Core.Services;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail.Events;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Jobs;
using FluentAssertions;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class AccountCreationFlowTests
{
    [Fact]
    public void Create_ShouldNotExposePasswordInAggregateOrDomainEvent()
    {
        var aggregate = AccountAggregate.Create(
            Guid.Parse("d63f0041-bc4c-4aad-aa50-9fa68735e8b7"),
            "user@example.com",
            "Test",
            "User",
            "user@example.com",
            ["member"],
            false);

        aggregate.GetType().GetProperty("Password").Should().BeNull();
        typeof(AccountCreatedDomainEvent).GetProperty("Password").Should().BeNull();

        aggregate.Events.Should().ContainSingle();
        var domainEvent = aggregate.Events.Single().Should().BeOfType<AccountCreatedDomainEvent>().Subject;
        domainEvent.Email.Should().Be("user@example.com");
        domainEvent.UserName.Should().Be("user@example.com");
        domainEvent.Roles.Should().ContainSingle("member");
    }

    [Fact]
    public async Task ConfirmEmailHandler_ShouldDecodeBase64UrlEncodedToken_AndEmitEmailConfirmedEvent()
    {
        var identityService = new CapturingIdentityService();
        var aggregateId = Guid.Parse("d63f0041-bc4c-4aad-aa50-9fa68735e8b7");
        var aggregate = AccountAggregate.Create(
            aggregateId,
            "user@example.com",
            "Test",
            "User",
            "user@example.com",
            ["member"],
            false);
        InitialiseServiceLocator(new StubEventsService(aggregate));
        var handler = new ConfirmEmailCommand.ConfirmEmailCommandHandler(identityService, new StubMessages());
        const string rawToken = "test-token/with+symbols=";
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

        var result = await handler.Handle(
            new ConfirmEmailCommand(encodedToken, "user@example.com"),
            CancellationToken.None);

        identityService.ConfirmedEmail.Should().Be("user@example.com");
        identityService.ConfirmedToken.Should().Be(rawToken);
        identityService.UserId.Should().Be(aggregateId);
        result.EmailConfirmed.Should().BeTrue();
        result.Events.Should().ContainSingle(e => e is AccountEmailConfirmedDomainEvent);
    }

    [Fact]
    public void ConfirmEmail_ShouldEmitDomainEventOnlyOnce()
    {
        var aggregate = AccountAggregate.Create(
            Guid.Parse("d63f0041-bc4c-4aad-aa50-9fa68735e8b7"),
            "user@example.com",
            "Test",
            "User",
            "user@example.com",
            ["member"],
            false);
        var initialEventCount = aggregate.Events.Count;

        aggregate.ConfirmEmail();
        aggregate.ConfirmEmail();

        aggregate.EmailConfirmed.Should().BeTrue();
        aggregate.Events.Should().HaveCount(initialEventCount + 1);
        aggregate.Events.Skip(initialEventCount).Should().ContainSingle(e => e is AccountEmailConfirmedDomainEvent);
    }

    [Fact]
    public async Task AccountCreatedApplicationEventHandler_ShouldQueueConfirmationEmail_WhenAccountIsNotConfirmed()
    {
        var jobRunner = new CapturingJobRunner();
        var handler = new AccountCreatedApplicationEventHandler(jobRunner);

        await handler.Handle(
            new AccountCreatedApplicationEvent("user@example.com", false),
            CancellationToken.None);

        jobRunner.Email.Should().Be("user@example.com");
        jobRunner.RunInvocations.Should().Be(1);
    }

    [Fact]
    public async Task AccountCreatedApplicationEventHandler_ShouldSkipConfirmationEmail_WhenAccountIsAlreadyConfirmed()
    {
        var jobRunner = new CapturingJobRunner();
        var handler = new AccountCreatedApplicationEventHandler(jobRunner);

        await handler.Handle(
            new AccountCreatedApplicationEvent("user@example.com", true),
            CancellationToken.None);

        jobRunner.Email.Should().BeNull();
        jobRunner.RunInvocations.Should().Be(0);
    }

    private sealed class CapturingIdentityService : IIdentityService
    {
        public string? ConfirmedToken { get; private set; }

        public string? ConfirmedEmail { get; private set; }

        public Guid UserId { get; } = Guid.Parse("d63f0041-bc4c-4aad-aa50-9fa68735e8b7");

        public Task<object?> AuthenticateAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

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
            CancellationToken cancellationToken = default)
        {
            ConfirmedToken = token;
            ConfirmedEmail = email;

            return Task.FromResult(IdentityResult.Success());
        }

        public Task<IdentityResult> ConfirmEmailChangeAsync(
            string currentEmail,
            string newEmail,
            string token,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<string> GetUserNameAsync(Guid userId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Guid?> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult<Guid?>(UserId);

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

    private sealed class StubEventsService : IEventsService<AccountAggregate, AccountAggregateId>
    {
        private readonly AccountAggregate _aggregate;

        public StubEventsService(AccountAggregate aggregate) => _aggregate = aggregate;

        public Task PersistAsync(AccountAggregate aggregate, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<AccountAggregate?> RehydrateAsync(
            AccountAggregateId key,
            CancellationToken cancellationToken = default) =>
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

    private sealed class CapturingJobRunner : IJobRunner
    {
        public string? Email { get; private set; }

        public int RunInvocations { get; private set; }

        public string Run<TJob, TJobOptions>(Action<TJobOptions>? configureJobOptions = null)
            where TJobOptions : class, IJobOptions
            where TJob : IJob<TJobOptions>
        {
            RunInvocations++;

            var options = Activator.CreateInstance<TJobOptions>();
            configureJobOptions?.Invoke(options);

            if (options is SendEmailConfirmJobOptions emailOptions)
                Email = emailOptions.Email;

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

        public string RunRecurring<TJob, TJobOptions>(
            string jobId,
            string cron,
            Action<TJobOptions>? configureJobOptions = null)
            where TJobOptions : class, IJobOptions
            where TJob : IJob<TJobOptions> => throw new NotSupportedException();

        public void TriggerRecurringJob(string id) => throw new NotSupportedException();

        public void RemoveRecurringJob(string id) => throw new NotSupportedException();
    }
}
