namespace Aviant.DDD.Infrastructure.Service
{
    using Application;

    public class DateTime : IDateTime
    {
        public System.DateTime Now { get; } = System.DateTime.Now;

        public System.DateTime UtcNow { get; } = System.DateTime.UtcNow;
    }
}