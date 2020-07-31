using System.Threading.Tasks;
using CleanArchitecture.Services;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Services.v1_0.Interfaces
{
    public interface ITodoListsService : IService
    {
        public Task<ActionResult<TodosVm>> Get();

        // public Task<FileResult> Get(int id);
        
        // public Task<ActionResult<int>> Create(CreateTodoListCommand command);
        //
        // public Task<ActionResult> Update(int id, UpdateTodoListCommand command);
        //
        // public Task<ActionResult> Delete(int id);
    }
}