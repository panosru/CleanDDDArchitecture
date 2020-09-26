namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.ConfirmEmail
{
    using System.Threading.Tasks;
    using Domains.Account.Application.UseCases.ConfirmEmail;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public sealed class Account
        : ApiController<ConfirmEmailUseCase, Account>,
          IConfirmEmailOutput
    {
        public Account([FromServices] ConfirmEmailUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IConfirmEmailOutput Members

        void IConfirmEmailOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void IConfirmEmailOutput.Ok() =>
            ViewModel = Ok();

        #endregion

        /// <summary>
        ///     Confirm user email with token taken from authentication endpoint
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("confirm/{Token}/{Email}")]
        public async Task<IActionResult> ConfirmEmail([FromRoute] ConfirmEmailCommand command)
        {
            await UseCase
               .SetInput(command)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}