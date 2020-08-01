using System;
using Aviant.DDD.Domain.Event;

namespace Aviant.DDD.Domain.Entity
{
    public abstract class Base<T> : HaveEvents, IEntity
    {
        public T Id { get; set; }
        
        public DateTime Created { get; private set; } = DateTime.Now;

        public bool IsActive { get; private set; } = true;

    }
}