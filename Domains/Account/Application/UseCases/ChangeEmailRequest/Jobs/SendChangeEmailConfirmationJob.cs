using System.Text.Encodings.Web;
using Aviant.Application.Email;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Shared.Core;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Serilog;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest.Jobs;

internal sealed class SendChangeEmailConfirmationJobOptions : IJobOptions
{
    public string CurrentEmail { get; set; } = string.Empty;

    public string NewEmail { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}

[Queue(JobQueue.Main)]
internal sealed class SendChangeEmailConfirmationJob : IJob<SendChangeEmailConfirmationJobOptions>
{
    private readonly AppSettings _appSettings;
    private readonly IEmailService _emailService;
    private readonly LinkGenerator _linkGenerator;

    public SendChangeEmailConfirmationJob(
        IOptions<AppSettings> appSettings,
        LinkGenerator linkGenerator,
        IEmailService emailService)
    {
        _appSettings = appSettings.Value;
        _linkGenerator = linkGenerator;
        _emailService = emailService;
    }

    public async Task PerformAsync(SendChangeEmailConfirmationJobOptions jobOptions)
    {
        var baseUri = new Uri(_appSettings.BaseUrl);
        var confirmationLink = _linkGenerator.GetUriByAction(
            action: "ConfirmEmailChange",
            controller: "Account",
            values: new
            {
                currentEmail = jobOptions.CurrentEmail,
                newEmail = jobOptions.NewEmail,
                token = jobOptions.Token
            },
            scheme: baseUri.Scheme,
            host: new HostString(baseUri.Host, baseUri.Port));

        var sent = await _emailService
            .To(jobOptions.FullName, jobOptions.NewEmail)
            .WithSubject("Confirm your new email")
            .WithBodyHtml(
                $"Please confirm your new email by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.")
            .SendAsync()
            .ConfigureAwait(false);

        if (sent)
            Log.Information("Email change confirmation sent to {Email}", jobOptions.NewEmail);
        else
            Log.Error("Email change confirmation failed to {Email}", jobOptions.NewEmail);
    }
}
