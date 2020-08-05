namespace Aviant.DDD.Domain.Entity
{
    using System.ComponentModel.DataAnnotations;
    using Event;

    public abstract class Base<T> : HaveEvents, IEntity
    {
        [Key]
        public T Id { get; set; }
    }
}