using Aviant.Core.Enum;
using CleanDDDArchitecture.Domains.Shared.Core.Identity;
using Hangfire.Dashboard;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation;

public sealed class HangfireAuthorizationFilter : LocalRequestsOnlyAuthorizationFilter
{
    /// <inheritdoc cref="LocalRequestsOnlyAuthorizationFilter" />
    public new bool Authorize(DashboardContext context)
    {
        var root = context.GetHttpContext().User.IsInRole(Roles.Root.ToString(StringCase.Lower));

        return base.Authorize(context) || root;
    }
}
