namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.Controllers.V1_0
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TodoList.Application.UseCases.Create;
    using TodoList.Application.UseCases.Delete;
    using TodoList.Application.UseCases.Export;
    using TodoList.Application.UseCases.Export.ViewModels;
    using TodoList.Application.UseCases.GetAll;
    using TodoList.Application.UseCases.GetAll.ViewModels;
    using TodoList.Application.UseCases.Update;

    /// <summary>
    ///     Todo Lists endpoints
    /// </summary>
    public class TodoLists : ApiController
    {
        /// <summary>
        ///     Get all todo lists with their items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get()
        {
            RequestResult requestResult = await Orchestrator.SendQuery(new GetTodosQuery());

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            var todosVm = requestResult.Payload<TodosVm>();

            return Ok(todosVm);
        }

        /// <summary>
        ///     Export todo list items into csv file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesDefaultResponseType]
        public async Task<FileResult> Get([FromRoute] int id)
        {
            RequestResult requestResult = await Orchestrator.SendQuery(new ExportTodosQuery { ListId = id });

            if (requestResult.Success)
            {
                var exportTodosVm = requestResult.Payload<ExportTodosVm>();

                return File(exportTodosVm.Content, exportTodosVm.ContentType, exportTodosVm.FileName);
            }

            return File(
                Encoding.ASCII.GetBytes(string.Join("\n\r", requestResult.Messages.ToArray())),
                "text/plain",
                "Error.txt");
        }

        /// <summary>
        ///     Create new todo list
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromBody] CreateTodoListCommand command)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return Ok(requestResult.Payload());
        }

        /// <summary>
        ///     Update a todo list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Update(
            [FromRoute] int                   id,
            [FromBody]  UpdateTodoListCommand command)
        {
            if (id != command.Id) return BadRequest();

            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return NoContent();
        }

        /// <summary>
        ///     Delete a todo list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(new DeleteTodoListCommand { Id = id });

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return NoContent();
        }
    }
}