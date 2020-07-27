using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Users.Commands.Authenticate;
using MediatR;

namespace CleanArchitecture.API.Controllers
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
    }
}