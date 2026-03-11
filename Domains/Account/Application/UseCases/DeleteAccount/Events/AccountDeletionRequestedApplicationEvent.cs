using Aviant.Application.ApplicationEvents;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount.Jobs;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount.Events;

internal sealed record AccountDeletionRequestedApplicationEvent(
    string Email,
    string FullName,
    string Token) : ApplicationEvent;

internal sealed class AccountDeletionRequestedApplicationEventHandler
    : ApplicationEventHandler<AccountDeletionRequestedApplicationEvent>
{
    private readonly IJobRunner _jobRunner;

    public AccountDeletionRequestedApplicationEventHandler(IJobRunner jobRunner) => _jobRunner = jobRunner;

    public override Task Handle(
        AccountDeletionRequestedApplicationEvent @event,
        CancellationToken cancellationToken)
    {
        _jobRunner.Run<SendAccountDeletionEmailJob, SendAccountDeletionEmailJobOptions>(
            options =>
            {
                options.Email = @event.Email;
                options.FullName = @event.FullName;
                options.Token = @event.Token;
            });

        return Task.CompletedTask;
    }
}
