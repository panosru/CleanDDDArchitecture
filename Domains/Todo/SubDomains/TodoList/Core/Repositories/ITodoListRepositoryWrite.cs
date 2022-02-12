namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;

using Aviant.Foundation.Core.Persistence;
using Todo.Core.Entities;

public interface ITodoListRepositoryWrite : IRepositoryWrite<TodoListEntity, int>
{ }
