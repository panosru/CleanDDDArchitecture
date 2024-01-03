using Hangfire;
using Lamar;

namespace CleanDDDArchitecture.Hosts.Worker;

internal class CommonRegistry : ServiceRegistry
{
    public CommonRegistry()
    {
        For<IBackgroundJobClient>().Use<BackgroundJobClient>();
    }
}
