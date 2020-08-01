namespace Aviant.DDD.Domain.Entity
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}