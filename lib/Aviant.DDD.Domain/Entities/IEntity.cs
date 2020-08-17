namespace Aviant.DDD.Domain.Entities
{
    using System.Threading.Tasks;

    public interface IEntity<T>
    {
        public T Id { get; set; }

        Task<bool> Validate();
    }
}