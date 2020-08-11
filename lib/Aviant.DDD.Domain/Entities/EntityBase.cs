namespace Aviant.DDD.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Events;

    public abstract class EntityBase<T> : HaveEvents, IEntity
    {
        [Key]
        public T Id { get; set; } = default!;
    }
}