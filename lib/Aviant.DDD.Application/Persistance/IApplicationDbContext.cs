namespace Aviant.DDD.Application.Persistance
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IApplicationDbContextTMP
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}