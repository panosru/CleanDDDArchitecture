namespace CleanDDDArchitecture.RestApi.Controllers.V1_0
{
    using System.Threading.Tasks;
    using Application.TodoLists.Commands.CreateTodoList;
    using Application.TodoLists.Commands.DeleteTodoList;
    using Application.TodoLists.Commands.UpdateTodoList;
    using Application.TodoLists.Queries.ExportTodos;
    using Application.TodoLists.Queries.GetTodos;
    using CleanDDDArchitecture.Services.v1_0.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// </summary>
    public class TodoLists : ApiController
    {
        private readonly ITodoListsService _todoListsService;

        /// <summary>
        /// </summary>
        /// <param name="todoListsService"></param>
        public TodoLists(ITodoListsService todoListsService)
        {
            _todoListsService = todoListsService;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<TodosVm>> Get()
        {
            return await _todoListsService.Get();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FileResult> Get(int id)
        {
            var vm = await Mediator.Send(new ExportTodosQuery {ListId = id});
            //
            return File(vm.Content, vm.ContentType, vm.FileName);
            // return await _todoListsService.Get(id);
        }

        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateTodoListCommand command)
        {
            return await Mediator.Send(command);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Update(int id, UpdateTodoListCommand command)
        {
            if (id != command.Id) return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteTodoListCommand {Id = id});

            return NoContent();
        }
    }
}