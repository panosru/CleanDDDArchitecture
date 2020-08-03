namespace Aviant.DDD.Domain
{
    using Interfaces;

    public static class ServiceLocator
    {
        public static IContainer Container { get; private set; }

        public static void Initialise(IContainer container)
        {
            Container = container;
        }
    }
}