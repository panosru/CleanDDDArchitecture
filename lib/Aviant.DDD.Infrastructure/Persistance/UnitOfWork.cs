namespace Aviant.DDD.Infrastructure.Persistance
{
    using System;
    using System.Threading.Tasks;
    using Application.Persistance;
    using Domain.Persistence;

    public class UnitOfWork<TDbContext> : IUnitOfWork, IDisposable
        where TDbContext : IApplicationDbContextBase
    {
        private readonly TDbContext _context;
        private bool _isDisposed;

        public UnitOfWork(TDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<int> Commit()
        {
            try
            {
                var affectedRows = await _context.SaveChangesAsync()
                    .ConfigureAwait(false);

                return affectedRows;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing) _context.Dispose();

            _isDisposed = true;
        }
    }
}