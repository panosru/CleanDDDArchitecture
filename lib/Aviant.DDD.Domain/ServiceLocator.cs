using Aviant.DDD.Domain.Interfaces;

namespace Aviant.DDD.Domain
{
    public static class ServiceLocator
    {
        public static IContainer Container { get; private set; }

        public static void Initialise(IContainer container)
        {
            Container = container;
        }
    }
}