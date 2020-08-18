namespace CleanDDDArchitecture.RestApi.Controllers
{
    using System.Threading.Tasks;
    using Application.Users.Commands.Authenticate;
    using Application.Users.Commands.ConfirmEmail;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// </summary>
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class User : ApiController
    {
        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> Authenticate(AuthenticateCommand command)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (requestResult.Success && !(requestResult.Payload() is null))
                return Ok(requestResult.Payload());

            return Unauthorized();
        }

        /// <summary>
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

            if (requestResult.Success)
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