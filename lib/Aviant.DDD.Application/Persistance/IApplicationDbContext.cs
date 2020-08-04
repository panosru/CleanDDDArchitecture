namespace Aviant.DDD.Application.Persistance
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}