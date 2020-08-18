namespace Aviant.DDD.Infrastructure.Persistance
{
    using System;
    using System.Threading.Tasks;
    using Application.Persistance;
    using Domain.Persistence;

    public class UnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : IApplicationDbContextBase
    {
        private readonly TDbContext _context;

        public UnitOfWork(TDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Commit()
        {
            try
            {
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}