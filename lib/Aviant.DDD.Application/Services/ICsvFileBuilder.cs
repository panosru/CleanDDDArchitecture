namespace Aviant.DDD.Application.Services
{
    using System.Collections.Generic;

    public interface ICsvFileBuilder<in TRecord>
        where TRecord : class
    {
        byte[] BuildTodoItemsFile(IEnumerable<TRecord> records);
    }
}