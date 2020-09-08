namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.Controllers.V1_0
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Domains.Account.Application.Aggregates;
    using Domains.Account.Application.UseCases.Create;
    using Domains.Account.Application.UseCases.Create.Dtos;
    using Domains.Account.Application.UseCases.UpdateDetails;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// </summary>
    [ApiVersion("1.0")]
    public sealed class Account : ApiController
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateAccountDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
                return BadRequest();

            var command = new CreateAccount(dto.FirstName, dto.LastName, dto.Email);
            // await Mediator.Send(command, cancellationToken);
            // return Ok(command);
            // return CreatedAtAction("GetFoobar", new {id = command.Id}, command);

            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return Ok(requestResult.Payload());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update(
            int               id,
            CreateAccountDto  dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                return BadRequest();

            var command = new UpdateAccount(
                new AccountId(id),
                dto.FirstName,
                dto.LastName,
                dto.Email);
            // await Mediator.Publish(command, cancellationToken);
            // return Ok(command);

            RequestResult requestResult = await Orchestrator.SendCommand(command);

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);

            return Ok(requestResult.Payload());
        }

        // [HttpGet, Route("{id:guid}", Name = "GetFoobar")]
        // public async Task<IActionResult> GetFoobar(Guid id, CancellationToken cancellationToken = default)
        // {
        //     var query = 
        // }
    }
}