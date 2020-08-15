namespace Aviant.DDD.Infrastructure.Persistance
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Persistance;
    using Domain.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class UnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : IApplicationDbContext
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
            catch (Exception e) //TODO: Create a custom domain exception for this case.
            {
                return false;
            }
        }
    }
}