namespace Aviant.DDD.Domain.Persistence
{
    using System.Threading.Tasks;

    public interface IUnitOfWork
    {
        public Task<bool> Commit();
    }
}