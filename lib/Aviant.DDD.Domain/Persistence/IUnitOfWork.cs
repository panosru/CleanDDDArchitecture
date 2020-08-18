namespace Aviant.DDD.Domain.Persistence
{
    using System.Threading.Tasks;

    /// <summary>
    /// Unit of Work Interface
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Commit changes to database
        /// </summary>
        /// <returns>Integer representing affected rows</returns>
        public Task<int> Commit();
    }
}