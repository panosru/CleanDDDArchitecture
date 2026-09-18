using Aviant.Application.Email;
using Aviant.Infrastructure.Email;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
/// Email service extension
/// </summary>
public static class Email
{
    /// <summary>
    /// Add email service
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddAviantEmail(new SmtpSettings
        {
            Host     = configuration["EmailSettings:SmtpHost"] ?? "localhost",
            Port     = configuration.GetValue("EmailSettings:SmtpPort", defaultValue: 1025),
            // Optional: Aspire and Compose point at Mailpit, which has no TLS.
            Security = configuration.GetValue("EmailSettings:EnableSsl", defaultValue: false)
                ? SmtpSecurity.SslOnConnect
                : SmtpSecurity.None,
            Username = configuration["EmailSettings:SmtpUsername"],
            Password = configuration["EmailSettings:SmtpPassword"],
            From     = new Mailbox(
                configuration["AppSettings:Emails:NoReply"] ?? "no-reply@localhost",
                configuration["AppSettings:Title"]),
        });
}
