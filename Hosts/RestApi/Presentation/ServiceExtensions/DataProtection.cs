using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///   DataProtection service extension
/// </summary>
public static class DataProtection
{
    /// <summary>
    ///     Add DataProtection services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDataProtectionServices(this IServiceCollection services)
    {
        var keysPath = Path.GetFullPath(
            Environment.GetEnvironmentVariable("DataProtection__KeysPath")
         ?? Environment.GetEnvironmentVariable("DATA_PROTECTION_KEYS_PATH")
         ?? Path.Combine(AppContext.BaseDirectory, "DataProtection-Keys"));

        services.AddDataProtection()
            .SetApplicationName("RestApi")
            .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
            .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

        return services;
    }
}
