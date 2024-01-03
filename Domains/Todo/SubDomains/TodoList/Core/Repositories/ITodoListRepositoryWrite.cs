using Aviant.Core.Persistence;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;

public interface ITodoListRepositoryWrite : IRepositoryWrite<TodoListEntity, int>;
