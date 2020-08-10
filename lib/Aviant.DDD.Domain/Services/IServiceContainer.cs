namespace Aviant.DDD.Domain.Services
{
    using System;

    public interface IServiceContainer
    {
        T GetService<T>(Type type);
    }
}