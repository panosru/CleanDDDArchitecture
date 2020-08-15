namespace Aviant.DDD.Domain.Persistence
{
    using System.Threading.Tasks;

    public interface IUnitOfWork //TODO: Write UnitOfWork with Write Repository
    {
        public Task<bool> Commit();
    }
}