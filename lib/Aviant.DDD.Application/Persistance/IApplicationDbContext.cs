using System.Threading;
using System.Threading.Tasks;

namespace Aviant.DDD.Application.Persistance
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}