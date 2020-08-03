namespace Aviant.DDD.Infrastructure.Persistance
{
    using System;
    using System.Threading.Tasks;
    using Domain.Persistence;
    using Microsoft.EntityFrameworkCore;

    public abstract class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public UnitOfWork(DbContext context)
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
#warning Create a custom domain exception for this case.
            catch (Exception e)
            {
                return false;
            }
        }
    }
}