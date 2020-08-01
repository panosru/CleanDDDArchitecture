using System.Threading.Tasks;

namespace Aviant.DDD.Domain.Persistence
{
    public interface IUnitOfWork
    {
        public Task<bool> Commit();
    }
}