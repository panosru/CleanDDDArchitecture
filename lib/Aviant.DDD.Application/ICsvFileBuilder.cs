using System.Collections.Generic;

namespace Aviant.DDD.Application
{
    public interface ICsvFileBuilder<in TRecord>
        where TRecord : class
    {
        byte[] BuildTodoItemsFile(IEnumerable<TRecord> records);
    }
}