using Aviant.Core.EventSourcing.DomainEvents;
using Aviant.Core.EventSourcing.EventBus;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Polly;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail.Events;

public sealed record AccountEmailConfirmedDomainEvent : DomainEvent<AccountAggregate, AccountAggregateId>
{
    #pragma warning disable 8618
    private AccountEmailConfirmedDomainEvent()
    { }
    #pragma warning restore 8618

    public AccountEmailConfirmedDomainEvent(AccountAggregate accountAggregate)
        : base(accountAggregate)
    {
        Email = accountAggregate.Email;
        EmailConfirmed = accountAggregate.EmailConfirmed;
    }

    public string Email { get; private set; }

    public bool EmailConfirmed { get; private set; }

    internal sealed class AccountEmailConfirmedDomainEventConsumer : DomainEventHandler<AccountEmailConfirmedDomainEvent>
    {
        public override Task Handle(
            EventReceived<AccountEmailConfirmedDomainEvent> @event,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public override IAsyncPolicy RetryPolicy() =>
            Policy
                .Handle<ArgumentOutOfRangeException>()
                .WaitAndRetryAsync(
                    3,
                    i => TimeSpan.FromSeconds(i));
    }
}
