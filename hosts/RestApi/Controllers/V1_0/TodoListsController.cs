namespace CleanArchitecture.RestApi.Controllers.V1_0
{
    using System.Threading.Tasks;
    using Application.TodoLists.Commands.CreateTodoList;
    using Application.TodoLists.Commands.DeleteTodoList;
    using Application.TodoLists.Commands.UpdateTodoList;
    using Application.TodoLists.Queries.ExportTodos;
    using Application.TodoLists.Queries.GetTodos;
    using CleanArchitecture.Services.v1_0.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class TodoListsController : ApiController
    {
        private readonly ITodoListsService _todoListsService;

        public TodoListsController(ITodoListsService todoListsService)
        {
            _todoListsService = todoListsService;
        }

        [HttpGet]
        public async Task<ActionResult<TodosVm>> Get()
        {
            return await _todoListsService.Get();
        }

        [HttpGet("{id}")]
        public async Task<FileResult> Get(int id)
        {
            var vm = await Mediator.Send(new ExportTodosQuery {ListId = id});
            //
            return File(vm.Content, vm.ContentType, vm.FileName);
            // return await _todoListsService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateTodoListCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateTodoListCommand command)
        {
            if (id != command.Id) return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteTodoListCommand {Id = id});

            return NoContent();
        }
    }
}