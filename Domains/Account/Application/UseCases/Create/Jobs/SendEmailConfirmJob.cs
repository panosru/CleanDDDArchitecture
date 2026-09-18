using System.Text;
using System.Text.Encodings.Web;
using Aviant.Application.Email;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Shared.Core;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Jobs;

internal sealed class SendEmailConfirmJobOptions : IJobOptions
{
    public string Email { get; set; }
}

[Queue(JobQueue.Main)]
internal sealed class SendEmailConfirmJob : IJob<SendEmailConfirmJobOptions>
{
    private readonly ILogger<SendEmailConfirmJob> _logger;
    private readonly UserManager<AccountUser> _userManager;
    private readonly AppSettings _appSettings;
    private readonly LinkGenerator _linkGenerator;
    private readonly IEmailService _emailService;

    public SendEmailConfirmJob(
        UserManager<AccountUser> userManager, 
        IOptions<AppSettings> appSettings,
        LinkGenerator linkGenerator,
        IEmailService            emailService,
        ILogger<SendEmailConfirmJob> logger)
    {
        _logger = logger;
        _userManager = userManager;
        _appSettings = appSettings.Value;
        _linkGenerator = linkGenerator;
        _emailService = emailService;
    }
    
    public async Task PerformAsync(SendEmailConfirmJobOptions jobOptions, CancellationToken cancellationToken)
    {
        // Get User by Email
        AccountUser? user = await _userManager.FindByEmailAsync(jobOptions.Email)
            .ConfigureAwait(false);
        
        // Check that user is not null
        if (user is null)
        {
            _logger.LogError("No user with email {Email}", jobOptions.Email);
            return;
        }

        var _code = await _userManager.GenerateEmailConfirmationTokenAsync(user)
            .ConfigureAwait(false);
        _code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(_code));

        Uri uri = new(_appSettings.BaseUrl);
        
        var confirmationLink = _linkGenerator.GetUriByAction(
            action: "ConfirmEmail",
            controller: "Account",
            values: new { token = _code, email = user.Email },
            scheme: uri.Scheme,
            host: new HostString(uri.Host, uri.Port));

        var email = await _emailService
            .To(user.FullName, user.Email)
            .WithSubject("Confirm your email")
            .WithBodyHtml(
                $"Please confirm your account by <a href='{HtmlEncoder
                    .Default.Encode(confirmationLink)}'>clicking here</a>.")
            .SendAsync()
            .ConfigureAwait(false);
        
        // If the email was send log success, otherwise report error log
        if (email)
            _logger.LogInformation("Email confirmation sent to {Email}", user.Email);
        else
            _logger.LogError("Email confirmation failed to {Email}", user.Email);
    }
}
