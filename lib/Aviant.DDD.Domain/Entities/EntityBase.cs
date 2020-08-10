namespace Aviant.DDD.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Events;

    public abstract class EntityBase<T> : HasEvents, IEntity
    {
        [Key]
        public T Id { get; set; }
    }
}