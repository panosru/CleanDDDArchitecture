namespace CleanDDDArchitecture.Hosts.RestApi.Application.Services
{
    #region

    using System;
    using System.Security.Claims;
    using Aviant.DDD.Application.Identity;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.JsonWebTokens;

    #endregion

    /// <summary>
    /// </summary>
    public class CurrentUser : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public CurrentUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        #region ICurrentUserService Members

        /// <summary>
        /// </summary>
        public Guid UserId
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User?
                   .FindFirstValue(JwtRegisteredClaimNames.Sub);

                return id is null
                    ? Guid.Empty
                    : Guid.Parse(id);
            }
        }

        #endregion
    }
}