namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.GetAll
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TodoList.Application.UseCases.GetAll;

    /// <summary>
    ///     Todo Lists endpoints
    /// </summary>
    public class TodoLists : ApiController<GetAllUseCase>, IGetAllOutput
    {
        public TodoLists([FromServices] GetAllUseCase useCase)
            : base(useCase)
        { }

        #region IGetAllOutput Members

        void IGetAllOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void IGetAllOutput.Ok(object? @object) =>
            ViewModel = Ok(@object);

        #endregion


        /// <summary>
        ///     Get all todo lists with their items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            await UseCase.ExecuteAsync(this);

            return ViewModel;
        }
    }
}