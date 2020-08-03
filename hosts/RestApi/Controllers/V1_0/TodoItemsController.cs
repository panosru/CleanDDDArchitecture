namespace CleanArchitecture.RestApi.Controllers.V1_0
{
    using System.Threading.Tasks;
    using Application.TodoItems.Commands.CreateTodoItem;
    using Application.TodoItems.Commands.DeleteTodoItem;
    using Application.TodoItems.Commands.UpdateTodoItem;
    using Application.TodoItems.Commands.UpdateTodoItemDetail;
    using Microsoft.AspNetCore.Mvc;

    public class TodoItemsController : ApiController
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateTodoItemCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateTodoItemCommand command)
        {
            if (id != command.Id) return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPut("[action]")]
        public async Task<ActionResult> UpdateItemDetails(int id, UpdateTodoItemDetailCommand command)
        {
            if (id != command.Id) return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteTodoItemCommand {Id = id});

            return NoContent();
        }
    }
}