namespace Aviant.DDD.Application
{
    using System;

    public interface IDateTime
    {
        DateTime Now { get; }
    }
}