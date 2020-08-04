namespace CleanDDDArchitecture.RestApi.Controllers
{
    using System.Threading.Tasks;
    using Application.Users.Commands.Authenticate;
    using Application.Users.Commands.ConfirmEmail;
    using Aviant.DDD.Application;
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
            var result = await Mediator.Send(command);

            if (null != result) return Ok(result);

            return Unauthorized();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("confirm/{token}/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Confirm([FromRoute] ConfirmEmailCommand command)
        {
            var result = await Mediator.Send(command);

            if (result.Succeeded) return Ok(result);

            return Forbid();
        }
    }
}