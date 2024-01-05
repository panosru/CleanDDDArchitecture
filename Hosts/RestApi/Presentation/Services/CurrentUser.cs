using System.Security.Claims;
using Aviant.Application.Identity;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Services;

/// <summary>
/// </summary>
public sealed class CurrentUser : ICurrentUserService
{
    /// <summary>
    /// </summary>
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
            var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.NameId);

            return id is null
                ? Guid.Empty
                : Guid.Parse(id);
        }
    }

    #endregion
}
