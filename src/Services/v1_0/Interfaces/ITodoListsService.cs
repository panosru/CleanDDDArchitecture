namespace CleanArchitecture.Services.v1_0.Interfaces
{
    using System.Threading.Tasks;
    using Application.TodoLists.Queries.GetTodos;
    using Microsoft.AspNetCore.Mvc;

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