namespace Aviant.DDD.Infrastructure.Service
{
    using System;
    using Application.Services;

    public class DateTimeService : IDateTimeService //TODO: Should be enriched with a plugin maybe? Also the folder should be renamed to Services
    {
        public DateTime Now { get; } = DateTime.Now;

        public DateTime UtcNow { get; } = DateTime.UtcNow;
    }
}