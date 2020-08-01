using System;

namespace Aviant.DDD.Domain.Interfaces
{
    public interface IContainer
    {
        T GetService<T>(Type type);
    }
}