namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.Authenticate
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Account.Application.UseCases.Authenticate;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [AllowAnonymous]
    [FeatureGate(Features.AccountAuthentication)]
    public sealed class Account
        : ApiController<AuthenticateUseCase, Account>,
          IAuthenticateOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public Account([FromServices] AuthenticateUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IAuthenticateOutput Members

        void IAuthenticateOutput.Ok(object? @object) =>
            ViewModel = Ok(@object);

        #endregion

        /// <summary>
        ///     Authenticate a user and a bearer or an email confirmation token
        /// </summary>
        /// <response code="200">Email confirmation required.</response>
        /// <response code="201">Login successful.</response>
        /// <response code="401">Invalid credentials or mail not confirmed.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Not found.</response>
        /// <param name="command"></param>
        /// <returns>Bearer token.</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateCommand command)
        {
            await UseCase
               .SetInput(command)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}