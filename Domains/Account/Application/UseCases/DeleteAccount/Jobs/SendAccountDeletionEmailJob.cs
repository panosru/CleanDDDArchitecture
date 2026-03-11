using System.Text.Encodings.Web;
using Aviant.Application.Email;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Shared.Core;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Serilog;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount.Jobs;

internal sealed class SendAccountDeletionEmailJobOptions : IJobOptions
{
    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}

[Queue(JobQueue.Main)]
internal sealed class SendAccountDeletionEmailJob : IJob<SendAccountDeletionEmailJobOptions>
{
    private readonly AppSettings _appSettings;
    private readonly IEmailService _emailService;
    private readonly LinkGenerator _linkGenerator;

    public SendAccountDeletionEmailJob(
        IOptions<AppSettings> appSettings,
        LinkGenerator linkGenerator,
        IEmailService emailService)
    {
        _appSettings = appSettings.Value;
        _linkGenerator = linkGenerator;
        _emailService = emailService;
    }

    public async Task PerformAsync(SendAccountDeletionEmailJobOptions jobOptions)
    {
        var baseUri = new Uri(_appSettings.BaseUrl);
        var confirmationLink = _linkGenerator.GetUriByAction(
            action: "ConfirmDelete",
            controller: "Account",
            values: new { token = jobOptions.Token, email = jobOptions.Email },
            scheme: baseUri.Scheme,
            host: new HostString(baseUri.Host, baseUri.Port));

        var sent = await _emailService
            .To(jobOptions.FullName, jobOptions.Email)
            .WithSubject("Confirm account deletion")
            .WithBodyHtml(
                $"Confirm account deletion by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.")
            .SendAsync()
            .ConfigureAwait(false);

        if (sent)
            Log.Information("Account deletion confirmation sent to {Email}", jobOptions.Email);
        else
            Log.Error("Account deletion confirmation failed to {Email}", jobOptions.Email);
    }
}
