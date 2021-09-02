namespace CleanDDDArchitecture.Hosts.RestApi.Application
{
    using Domains.Shared.Core.Identity;
    using Hangfire.Dashboard;

    public sealed class HangfireAuthorizationFilter : LocalRequestsOnlyAuthorizationFilter
    {
        /// <inheritdoc cref="LocalRequestsOnlyAuthorizationFilter" />
        public new bool Authorize(DashboardContext context)
        {
            var admin = context.GetHttpContext().User.IsInRole(Roles.Root);

            return base.Authorize(context) || admin;
        }
    }
}