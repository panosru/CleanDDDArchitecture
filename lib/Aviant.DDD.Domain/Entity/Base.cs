namespace Aviant.DDD.Domain.Entity
{
    using System;
    using Event;

    public abstract class Base<T> : HaveEvents, IEntity
    {
        public T Id { get; set; }

        public DateTime Created { get; } = DateTime.Now;

        public bool IsActive { get; } = true;
    }
}