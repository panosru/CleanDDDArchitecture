using System;
using System.Threading.Tasks;
using Aviant.DDD.Domain.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aviant.DDD.Infrastructure.Persistance
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        private DbContext _context;

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