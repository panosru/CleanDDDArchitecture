using CleanDDDArchitecture.Domains.Shared.Core.Identity;
using Hangfire.Dashboard;

namespace CleanDDDArchitecture.Hosts.RestApi.Application;

public sealed class HangfireAuthorizationFilter : LocalRequestsOnlyAuthorizationFilter
{
    /// <inheritdoc cref="LocalRequestsOnlyAuthorizationFilter" />
    public new bool Authorize(DashboardContext context)
    {
        var root = context.GetHttpContext().User.IsInRole(Roles.Root);

        return base.Authorize(context) || root;
    }
}
