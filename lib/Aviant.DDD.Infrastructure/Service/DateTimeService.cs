namespace Aviant.DDD.Infrastructure.Service
{
    using System;
    using Application.Services;

    public class DateTimeService : IDateTimeService
    {
        public DateTime Now { get; } = DateTime.Now;

        public DateTime UtcNow { get; } = DateTime.UtcNow;
    }
}