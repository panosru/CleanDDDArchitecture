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
        IConfiguration configuration)
    {
        // Get the configuration properties
        var smtpHost = configuration["EmailSettings:SmtpHost"];
        var smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
        var enableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"]);
        var smtpUsername = configuration["EmailSettings:SmtpUsername"];
        var smtpPassword = configuration["EmailSettings:SmtpPassword"];
        var fromName = configuration["EmailSettings:FromName"];
        var fromEmail = configuration["EmailSettings:FromEmail"];
        
        services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>(
            provider => new SmtpClientFactory(
                smtpHost,
                smtpPort,
                enableSsl,
                smtpUsername,
                smtpPassword));
        
        services.AddTransient<IEmailService, EmailService>(
            provider =>
            {
                var smtpClientFactory = provider.GetRequiredService<ISmtpClientFactory>();

                return new EmailService(smtpClientFactory, fromName, fromEmail);
            });
        
        return services;
    }
}
