using System.Security.Claims;
using Aviant.Application.Identity;
using Microsoft.AspNetCore.Http;

namespace CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Services;

public sealed class CurrentUser : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? _httpContextAccessor.HttpContext?.User.FindFirst("nameid")?.Value;

            return id is null ? Guid.Empty : Guid.Parse(id);
        }
    }
}
