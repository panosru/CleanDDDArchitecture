namespace CleanDDDArchitecture.Services.v1_0
{
    using System.Threading.Tasks;
    using Application.TodoLists.Queries.GetTodos;
    using Interfaces;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    public class TodoListsService : BaseService, ITodoListsService
    {
        public TodoListsService(IMediator mediator)
            : base(mediator)
        {
        }


        public async Task<ActionResult<TodosVm>> Get()
        {
            return await Mediator.Send(new GetTodosQuery());
        }

        // public async Task<FileResult> Get(int id)
        // {
        //     var vm = await _mediator.Send(new ExportTodosQuery {ListId = id});
        //
        //     return File(vm.Content, vm.ContentType, vm.FileName);
        // }
    }
}