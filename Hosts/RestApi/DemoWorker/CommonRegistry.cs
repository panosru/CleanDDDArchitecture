namespace DemoWorker;

using Hangfire;
using Lamar;

internal class CommonRegistry : ServiceRegistry
{
    public CommonRegistry()
    {
        For<IBackgroundJobClient>().Use<BackgroundJobClient>();
    }
}
