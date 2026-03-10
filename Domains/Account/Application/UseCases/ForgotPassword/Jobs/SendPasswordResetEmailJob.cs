using System.Text.Encodings.Web;
using Aviant.Application.Email;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Shared.Core;
using Hangfire;
using Microsoft.Extensions.Options;
using Serilog;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword.Jobs;

internal sealed class SendPasswordResetEmailJobOptions : IJobOptions
{
    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}

[Queue(JobQueue.Main)]
internal sealed class SendPasswordResetEmailJob : IJob<SendPasswordResetEmailJobOptions>
{
    private readonly AppSettings _appSettings;
    private readonly IEmailService _emailService;

    public SendPasswordResetEmailJob(
        IOptions<AppSettings> appSettings,
        IEmailService emailService)
    {
        _appSettings = appSettings.Value;
        _emailService = emailService;
    }

    public async Task PerformAsync(SendPasswordResetEmailJobOptions jobOptions)
    {
        var apiBaseUrl = _appSettings.BaseUrl.TrimEnd('/');
        var instructions = $$"""
            We received a request to reset your password.
            Submit the following JSON payload to POST {{apiBaseUrl}}/api/identity/reset-password:
            <pre>{
              "email": "{{HtmlEncoder.Default.Encode(jobOptions.Email)}}",
              "token": "{{HtmlEncoder.Default.Encode(jobOptions.Token)}}",
              "password": "&lt;your new password&gt;"
            }</pre>
            If you did not request a password reset, you can ignore this email.
            """;

        var sent = await _emailService
            .To(jobOptions.FullName, jobOptions.Email)
            .WithSubject("Reset your password")
            .WithBodyHtml(instructions)
            .SendAsync()
            .ConfigureAwait(false);

        if (sent)
            Log.Information("Password reset instructions sent to {Email}", jobOptions.Email);
        else
            Log.Error("Password reset instructions failed to {Email}", jobOptions.Email);
    }
}
