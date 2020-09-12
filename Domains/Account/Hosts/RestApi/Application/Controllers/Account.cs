namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.Controllers
{
    #region

    using System.Threading.Tasks;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Orchestration;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Controllers;
    using Domains.Account.Application.UseCases.Authenticate;
    using Domains.Account.Application.UseCases.ConfirmEmail;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public sealed class Account : ApiController
    {
        /// <summary>
        ///     Authenticate a user and a bearer or an email confirmation token
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> Authenticate([FromBody] AuthenticateCommand command)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (requestResult.Succeeded
             && !(requestResult.Payload() is null))
                return Ok(requestResult.Payload());

            return Unauthorized();
        }

        /// <summary>
        ///     Confirm user email with token taken from authentication endpoint
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("confirm/{Token}/{Email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdentityResult>> Confirm([FromRoute] ConfirmEmailCommand command)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (requestResult.Succeeded)
            {
                var identityResult = requestResult.Payload<IdentityResult>();

                if (identityResult.Succeeded)
                    return Ok();

                requestResult.Messages.AddRange(identityResult.Errors);
            }

            return BadRequest(requestResult.Messages);
        }
    }
}