using CleanDDDArchitecture.Domains.Account.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.CrossCutting;
using Serilog;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Setup;

/// <summary>
///  Database setup
/// </summary>
public class DatabaseSetup
{
    /// <summary>
    ///   Migrate and seed database
    /// </summary>
    /// <param name="services"></param>
    public async Task MigrateAndSeedDatabase(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        try
        {
            await AccountCrossCutting.GenerateDefaultUserIfNotExistsAsync(serviceProvider)
                .ConfigureAwait(false);

            await TodoCrossCutting.GenerateTodoMigrationsIfNewExistsAsync(serviceProvider)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while migrating or seeding the database");
            throw;
        }
    }
}
