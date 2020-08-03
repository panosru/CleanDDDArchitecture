namespace CleanArchitecture.RestApi.Services
{
    using System;
    using System.Security.Claims;
    using Aviant.DDD.Application.Identity;
    using Microsoft.AspNetCore.Http;

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
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                return id is null ? Guid.Empty : Guid.Parse(id);
            }
        }
    }
}