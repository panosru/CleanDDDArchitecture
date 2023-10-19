using Hangfire;
using Lamar;

namespace DemoWorker;

internal class WorkerActivator : JobActivator
{
    private readonly IContainer _container;

    public WorkerActivator(IContainer container) => _container = container;

    public override object ActivateJob(Type jobType) => _container.GetInstance(jobType);
}
