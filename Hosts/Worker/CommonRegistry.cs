namespace CleanDDDArchitecture.Hosts.Worker;

using Hangfire;
using Lamar;

internal class CommonRegistry : ServiceRegistry
{
    public CommonRegistry()
    {
        For<IBackgroundJobClient>().Use<BackgroundJobClient>();
    }
}
