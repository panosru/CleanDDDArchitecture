using Aviant.Infrastructure.CrossCutting;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Setup;

/// <summary>
///   Configuration setup
/// </summary>
public class ConfigurationSetup
{
    /// <summary>
    ///  Setup configuration
    /// </summary>
    /// <param name="builder"></param>
    public void Setup(WebApplicationBuilder builder)
    {
        // Your existing SetupConfiguration code here
        DependencyInjectionRegistry.ConfigurationBuilder = builder.Configuration;

        string[] configFiles =
        {
            "appsettings.yml",
            $"appsettings.{builder.Environment.EnvironmentName}.yml",
            "appsettings.yaml",
            $"appsettings.{builder.Environment.EnvironmentName}.yaml"
        };

        foreach (var configFile in configFiles
                     .Where(
                         configFile =>
                             File.Exists(
                                 Path.Combine(Directory.GetCurrentDirectory(), configFile))))
            builder.Configuration.AddYamlFile(configFile, false, true);

        MoveEnvironmentVariablesToEnd(builder.Configuration);

        DependencyInjectionRegistry.SetConfiguration(builder.Configuration);
    }

    /// <summary>
    ///   Move EnvironmentVariablesConfigurationSource to the end of the configuration sources
    /// </summary>
    /// <param name="configurationBuilder"></param>
    private void MoveEnvironmentVariablesToEnd(IConfigurationBuilder configurationBuilder)
    {
        // Items that needs to be moved to the end (FIFO)
        List<IConfigurationSource?> fifo = new()
        {
            // Get EnvironmentVariablesConfigurationSource item
            configurationBuilder.Sources
                .FirstOrDefault(
                    i =>
                        i.GetType() == typeof(EnvironmentVariablesConfigurationSource)),

            // Get CommandLineConfigurationSource item
            configurationBuilder.Sources
                .FirstOrDefault(
                    i =>
                        i.GetType() == typeof(CommandLineConfigurationSource)),

            // Get CommandLineConfigurationSource item
            configurationBuilder.Sources
                .FirstOrDefault(
                    i =>
                        i.GetType() == typeof(ChainedConfigurationSource))
        };

        fifo.ForEach(
            item =>
            {
                if (item is null)
                    return;

                // Move to the end
                configurationBuilder.Sources.Remove(item);
                configurationBuilder.Sources.Add(item);
            });
    }
}
