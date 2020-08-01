using System;

namespace Aviant.DDD.Application
{
    public interface IDateTime
    {
        DateTime Now { get; }
    }
}