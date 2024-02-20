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
using Serilog;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Jobs;

internal sealed class SendEmailConfirmJobOptions : IJobOptions
{
    public string Email { get; set; }
}

[Queue(JobQueue.Main)]
internal sealed class SendEmailConfirmJob : IJob<SendEmailConfirmJobOptions>
{
    private readonly UserManager<AccountUser> _userManager;
    private readonly AppSettings _appSettings;
    private readonly LinkGenerator _linkGenerator;
    private readonly IEmailService _emailService;

    public SendEmailConfirmJob(
        UserManager<AccountUser> userManager, 
        IOptions<AppSettings> appSettings,
        LinkGenerator linkGenerator,
        IEmailService            emailService)
    {
        _userManager = userManager;
        _appSettings = appSettings.Value;
        _linkGenerator = linkGenerator;
        _emailService = emailService;
    }
    
    public async Task PerformAsync(SendEmailConfirmJobOptions jobOptions)
    {
        // Get User by Email
        AccountUser? user = await _userManager.FindByEmailAsync(jobOptions.Email)
            .ConfigureAwait(false);
        
        // Check that user is not null
        if (user is null)
        {
            Log.Error($"User with Email {jobOptions.Email} not found!");
            return;
        }

        var _code = await _userManager.GenerateEmailConfirmationTokenAsync(user)
            .ConfigureAwait(false);
        _code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(_code));

        Uri uri = new(_appSettings.BaseUrl);
        
        var confirmationLink = _linkGenerator.GetUriByAction(
            action: "ConfirmEmail",
            controller: "Account",
            values: new { userId = user.Id, code = _code },
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
            Log.Information($"Email confirmation sent to {user.Email}");
        else
            Log.Error($"Email confirmation failed to {user.Email}");
    }
}
