﻿namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.Update
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TodoList.Application.UseCases.Update;

    /// <summary>
    ///     Todo Lists endpoints
    /// </summary>
    public class TodoLists : ApiController<UpdateTodoListUseCase>, IUpdateTodoListOutput
    {
        public TodoLists([FromServices] UpdateTodoListUseCase useCase)
            : base(useCase)
        { }

        #region IUpdateTodoListOutput Members

        void IUpdateTodoListOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        #endregion


        /// <summary>
        ///     Update a todo list
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(
            [FromRoute] int               id,
            [FromBody]  UpdateTodoListDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            await UseCase.ExecuteAsync(this, dto)
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}