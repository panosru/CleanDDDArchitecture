namespace Aviant.DDD.Domain.Entity
{
    using Event;

    public abstract class Base<T> : HaveEvents, IEntity
    {
        public T Id { get; set; }
    }
}