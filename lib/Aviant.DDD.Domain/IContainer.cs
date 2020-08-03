namespace Aviant.DDD.Domain.Interfaces
{
    using System;

    public interface IContainer
    {
        T GetService<T>(Type type);
    }
}