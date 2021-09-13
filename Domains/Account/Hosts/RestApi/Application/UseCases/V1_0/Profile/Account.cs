namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.Profile
{
    using System.Net.Mime;
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Account.Application.Identity;
    using Domains.Account.Application.UseCases.Profile;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <inheritdoc
    ///     cref="CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.ApiController&lt;TUseCase,TUseCaseOutput&gt;" />
    [ApiVersion("1.0")]
    [FeatureGate(Features.AccountProfile)]
    public sealed class Account
        : ApiController<ProfileAccountUseCase, Account>,
          IProfileAccountOutput
    {
        /// <inheritdoc />
        public Account([FromServices] ProfileAccountUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IProfileAccountOutput Members

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        void IProfileAccountOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        /// <summary>
        /// </summary>
        /// <param name="accountUser"></param>
        void IProfileAccountOutput.Ok(AccountUser accountUser) =>
            ViewModel = Ok(new AccountProfileResponse(accountUser));

        #endregion

        // void IUpdateDetailsOutput.Ok(AccountAggregate accountAggregate) =>
        //     ViewModel = Ok(new AccountGetByResponse(accountAggregate));

        /// <summary>
        ///     Get Current Account Profile
        /// </summary>
        /// <response code="200">The account.</response>
        /// <response code="404">Not Found.</response>
        /// <returns>Account Profile data.</returns>
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountProfileResponse))]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Get))]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetAccountProfile()
        {
            await UseCase.ExecuteAsync()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}