// ReSharper disable MemberCanBeInternal

using Aviant.Core.EventSourcing.DomainEvents;
using Aviant.Core.EventSourcing.EventBus;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Polly;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmail.Events;

public sealed record AccountEmailChangedDomainEvent : DomainEvent<AccountAggregate, AccountAggregateId>
{
    #pragma warning disable 8618
    private AccountEmailChangedDomainEvent()
    { }
    #pragma warning restore 8618

    public AccountEmailChangedDomainEvent(AccountAggregate accountAggregate)
        : base(accountAggregate)
    {
        UserName = accountAggregate.UserName;
        Email = accountAggregate.Email;
        EmailConfirmed = accountAggregate.EmailConfirmed;
    }

    public string UserName { get; private set; }

    public string Email { get; private set; }

    public bool EmailConfirmed { get; private set; }

    internal sealed class AccountEmailChangedDomainEventConsumer : DomainEventHandler<AccountEmailChangedDomainEvent>
    {
        public override Task Handle(
            EventReceived<AccountEmailChangedDomainEvent> @event,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public override IAsyncPolicy RetryPolicy() =>
            Policy
                .Handle<ArgumentOutOfRangeException>()
                .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(i));
    }
}
