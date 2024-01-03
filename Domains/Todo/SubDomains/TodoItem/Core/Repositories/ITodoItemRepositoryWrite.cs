using Aviant.Core.Persistence;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories;

public interface ITodoItemRepositoryWrite : IRepositoryWrite<TodoItemEntity, int>
{ }
