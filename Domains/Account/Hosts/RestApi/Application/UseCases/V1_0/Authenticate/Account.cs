namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.Authenticate
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Controllers;
    using Domains.Account.Application.UseCases.Authenticate;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public sealed class Account : ApiController<AuthenticateUseCase>, IAuthenticateOutput
    {
        public Account([FromServices] AuthenticateUseCase useCase)
            : base(useCase)
        { }

        #region IAuthenticateOutput Members

        void IAuthenticateOutput.Ok(object? @object) =>
            ViewModel = Ok(@object);

        #endregion

        /// <summary>
        ///     Authenticate a user and a bearer or an email confirmation token
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateCommand command)
        {
            await UseCase.ExecuteAsync(this, command)
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}