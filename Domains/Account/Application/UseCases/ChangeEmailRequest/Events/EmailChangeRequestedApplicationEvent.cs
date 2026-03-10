using Aviant.Application.ApplicationEvents;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest.Jobs;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest.Events;

internal sealed record EmailChangeRequestedApplicationEvent(
    string CurrentEmail,
    string NewEmail,
    string FullName,
    string Token) : ApplicationEvent;

internal sealed class EmailChangeRequestedApplicationEventHandler
    : ApplicationEventHandler<EmailChangeRequestedApplicationEvent>
{
    private readonly IJobRunner _jobRunner;

    public EmailChangeRequestedApplicationEventHandler(IJobRunner jobRunner) => _jobRunner = jobRunner;

    public override Task Handle(
        EmailChangeRequestedApplicationEvent @event,
        CancellationToken cancellationToken)
    {
        _jobRunner.Run<SendChangeEmailConfirmationJob, SendChangeEmailConfirmationJobOptions>(
            options =>
            {
                options.CurrentEmail = @event.CurrentEmail;
                options.NewEmail = @event.NewEmail;
                options.FullName = @event.FullName;
                options.Token = @event.Token;
            });

        return Task.CompletedTask;
    }
}
