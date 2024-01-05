using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace CleanDDDArchitecture.Hosts.WebApp.Presentation.ServiceExtensions;

/// <summary>
///  This class contains the security services.
/// </summary>
public static class Security
{
    /// <summary>
    ///   This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DataProtection-Keys")))
            .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                })
            .SetDefaultKeyLifetime(TimeSpan.FromDays(36500));
        
        services.AddCors();

        services.AddHsts(
            options =>
            {
                options.Preload           = true;
                options.IncludeSubDomains = true;
                options.MaxAge            = TimeSpan.FromDays(60);
            });

        services.AddAntiforgery(
            options =>
            {
                options.HeaderName    = "X-CSRF-TOKEN";
                options.FormFieldName = "AntiForgeryField";
            });
        
        services.Configure<CookiePolicyOptions>(
            options =>
            {
                options.CheckConsentNeeded    = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        
        return services;
    }
}
