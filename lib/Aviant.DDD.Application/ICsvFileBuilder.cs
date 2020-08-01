using System.Collections.Generic;

namespace Aviant.DDD.Application
{
    public interface ICsvFileBuilder<T>
        where T : class
    {
        byte[] BuildTodoItemsFile(IEnumerable<T> records);
    }
}