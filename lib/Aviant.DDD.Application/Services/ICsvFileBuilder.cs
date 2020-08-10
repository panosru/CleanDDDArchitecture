namespace Aviant.DDD.Application
{
    using System.Collections.Generic;

    public interface ICsvFileBuilder<in TRecord>
        where TRecord : class
    {
        byte[] BuildTodoItemsFile(IEnumerable<TRecord> records);
    }
}