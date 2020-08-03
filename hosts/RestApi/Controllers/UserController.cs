namespace CleanArchitecture.RestApi.Controllers
{
    using System.Threading.Tasks;
    using Application.Users.Commands.Authenticate;
    using Application.Users.Commands.ConfirmEmail;
    using Aviant.DDD.Application;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// </summary>
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiController]
    public class UserController : ApiController
    {
        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<object>> Authenticate(AuthenticateCommand command)
        {
            var result = await Mediator.Send(command);

            if (null != result) return Ok(result);

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpGet("confirm/{token}/{email}")]
        public async Task<ActionResult<Result>> Confirm([FromRoute] ConfirmEmailCommand command)
        {
            var result = await Mediator.Send(command);

            if (result.Succeeded) return Ok(result);

            return Forbid();
        }
    }
}