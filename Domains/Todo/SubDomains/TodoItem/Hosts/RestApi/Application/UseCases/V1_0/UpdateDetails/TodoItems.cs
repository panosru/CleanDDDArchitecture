namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.UpdateDetails
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TodoItem.Application.UseCases.UpdateDetails;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    public class TodoItems : ApiController<TodoItemUpdatedetailsUseCase>, ITodoItemUpdateDetailsOutput
    {
        public TodoItems([FromServices] TodoItemUpdatedetailsUseCase useCase)
            : base(useCase)
        { }

        #region ITodoItemUpdateDetailsOutput Members

        void ITodoItemUpdateDetailsOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        #endregion


        /// <summary>
        ///     Update todo item details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateDetails(
            [FromQuery] int                      id,
            [FromBody]  TodoItemUpdateDetailsDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            await UseCase.ExecuteAsync(this, dto)
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}