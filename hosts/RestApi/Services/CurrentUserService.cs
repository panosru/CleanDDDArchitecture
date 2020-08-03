namespace CleanArchitecture.RestApi.Services
{
    using System;
    using System.Security.Claims;
    using Aviant.DDD.Application.Identity;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.JsonWebTokens;

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue(JwtRegisteredClaimNames.Sub);

                return id is null ? Guid.Empty : Guid.Parse(id);
            }
        }
    }
}