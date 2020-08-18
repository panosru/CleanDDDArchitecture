using Microsoft.AspNetCore.Http;

namespace CleanDDDArchitecture.RestApi.Controllers.V1_0
{
    using System.Threading.Tasks;
    using Application.TodoItems.Commands.CreateTodoItem;
    using Application.TodoItems.Commands.DeleteTodoItem;
    using Application.TodoItems.Commands.GetTodoItem;
    using Application.TodoItems.Commands.UpdateTodoItem;
    using Application.TodoItems.Commands.UpdateTodoItemDetail;
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// </summary>
    public class TodoItems : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(System.Collections.Generic.List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get([FromRoute] GetTodoItemQuery query)
        {
            RequestResult requestResult = await Orchestrator.SendQuery(query);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);
            
            return Ok(requestResult.Payload());
        }

        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(System.Collections.Generic.List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(CreateTodoItemCommand command)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);
            
            return Ok(requestResult.Payload());
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(System.Collections.Generic.List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(int id, UpdateTodoItemCommand command)
        {
            if (id != command.Id) return BadRequest();

            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);
            
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("[action]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(System.Collections.Generic.List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateItemDetails(int id, UpdateTodoItemDetailCommand command)
        {
            if (id != command.Id) return BadRequest();

            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);
            
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(System.Collections.Generic.List<string>), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(new DeleteTodoItemCommand {Id = id});

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);
            
            return NoContent();
        }
    }
}