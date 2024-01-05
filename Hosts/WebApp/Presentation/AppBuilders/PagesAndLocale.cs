using CleanDDDArchitecture.Hosts.WebApp.Core.Localisation;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;

namespace CleanDDDArchitecture.Hosts.WebApp.Presentation.AppBuilders;

/// <summary>
///  Pages and locale app builder
/// </summary>
public static class PagesAndLocale
{
    /// <summary>
    ///  Use application builders in development environment
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UsePagesAndLocaleBuilder(this IApplicationBuilder app)
    {
        app
            .UseHttpsRedirection()
            .UseStaticFiles()
            .UseStatusCodePagesWithReExecute("/errors/{0}")
            .UseRouting()
            .UseRequestLocalization(
                app.ApplicationServices
                    .GetRequiredService<IOptions<RequestLocalizationOptions>>()
                    .Value);
        
        // Attempt to make auto-redirect to culture if it is not exist in the url
        // app.UseMiddleware<RedirectUnsupportedCulturesMiddleware>(new object[] {false});
        RewriteOptions rewriter = new();

        rewriter.Add(
            new RedirectUnsupportedCulturesRule(
                app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>(),
                app.ApplicationServices.GetService<LinkGenerator>()));
        app.UseRewriter(rewriter);
        
        return app;
    }
}
