using Aviant.Application.Email;
using Aviant.Infrastructure.Email;
using System.Globalization;

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
        IConfiguration configuration)
    {
        services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>(
            provider => new SmtpClientFactory(
                configuration["EmailSettings:SmtpHost"],
                int.Parse(configuration["EmailSettings:SmtpPort"]!, CultureInfo.InvariantCulture),
                // Optional: Aspire and Compose point at Mailpit, which has no TLS.
                configuration.GetValue("EmailSettings:EnableSsl", defaultValue: false),
                configuration["EmailSettings:SmtpUsername"],
                configuration["EmailSettings:SmtpPassword"]));
        
        services.AddTransient<IEmailService, EmailService>(
            provider =>
            {
                var smtpClientFactory = provider.GetRequiredService<ISmtpClientFactory>();

                return new EmailService(smtpClientFactory,
                    configuration["AppSettings:Title"],
                    configuration["AppSettings:Emails:NoReply"]);
            });
        
        return services;
    }
}
