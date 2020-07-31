using System.IO;
using System.Threading.Tasks;
using CleanArchitecture.Services.v1_0.Interfaces;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Services.v1_0
{
    public class TodoListsService : BaseService, ITodoListsService
    {

        public TodoListsService(IMediator mediator)
            : base(mediator)
        {
        }


        public async Task<ActionResult<TodosVm>> Get()
        {
            return await _mediator.Send(new GetTodosQuery());
        }

        // public async Task<FileResult> Get(int id)
        // {
        //     var vm = await _mediator.Send(new ExportTodosQuery {ListId = id});
        //
        //     return File(vm.Content, vm.ContentType, vm.FileName);
        // }
    }
}