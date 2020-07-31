using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Users.Commands.Authenticate;
using CleanArchitecture.Application.Users.Commands.ConfirmEmail;

namespace CleanArchitecture.REST.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiController]
    public class UserController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<object>> Authenticate(AuthenticateCommand command)
        {
            var result = await Mediator.Send(command);

            if (null != result)
            {
                return Ok(result);
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpGet("confirm/{token}/{email}")]
        public async Task<ActionResult<Result>> Confirm([FromRoute] ConfirmEmailCommand command)
        {
            var result = await Mediator.Send(command);
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return Forbid();
        }
    }
}