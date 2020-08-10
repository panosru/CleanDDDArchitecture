namespace Aviant.DDD.Application
{
    using System;

    public interface IDateTimeService
    {
        DateTime Now { get; }
        
        DateTime UtcNow { get; }
    }
}