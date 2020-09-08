namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.Controllers.V1_0
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TodoItem.Application.UseCases.Create;
    using TodoItem.Application.UseCases.Delete;
    using TodoItem.Application.UseCases.GetBy;
    using TodoItem.Application.UseCases.Update;
    using TodoItem.Application.UseCases.UpdateDetails;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    public class TodoItems : ApiController
    {
        /// <summary>
        ///     Get a todo item
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get([FromRoute] GetTodoItemQuery query)
        {
            RequestResult requestResult = await Orchestrator.SendQuery(query);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return Ok(requestResult.Payload());
        }

        /// <summary>
        ///     Create a new todo item
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromBody] CreateTodoItemCommand command)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return Ok(requestResult.Payload());
        }

        /// <summary>
        ///     Update todo item name and status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(
            [FromRoute] int                   id,
            [FromBody]  UpdateTodoItemCommand command)
        {
            if (id != command.Id) return BadRequest();

            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return NoContent();
        }

        /// <summary>
        ///     Update todo item details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("[action]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateItemDetails(
            [FromQuery] int                         id,
            [FromBody]  UpdateTodoItemDetailCommand command)
        {
            if (id != command.Id) return BadRequest();

            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return NoContent();
        }

        /// <summary>
        ///     Delete a todo item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(new DeleteTodoItemCommand { Id = id });

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return NoContent();
        }
    }
}