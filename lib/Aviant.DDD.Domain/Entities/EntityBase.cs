namespace Aviant.DDD.Domain.Entities
{
    using Events;

    public abstract class EntityBase<T> : HaveEvents, IEntity<T>
    {
        public T Id { get; set; } = default!;
    }
}