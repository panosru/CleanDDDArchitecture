using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace CleanDDDArchitecture.Hosts.WebApp.Core.Localisation;

public sealed class RedirectUnsupportedCulturesRule : IRule
{
    /// <summary>
    /// 
    /// </summary>
    private readonly IList<CultureInfo> _cultureItems;

    /// <summary>
    /// 
    /// </summary>
    private readonly string _cultureRouteKey;

    private readonly LinkGenerator _linkGenerator;

    public RedirectUnsupportedCulturesRule(IOptions<RequestLocalizationOptions> options, LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;

        RouteDataRequestCultureProvider provider = options.Value.RequestCultureProviders
           .OfType<RouteDataRequestCultureProvider>()
           .First();

        _cultureItems = options.Value.SupportedUICultures;

        _cultureRouteKey = provider.RouteDataStringKey;
    }

    public void ApplyRule(RewriteContext rewriteContext)
    {
        //TODO: find why non-existing resources that would give 404 (such as missing ico) send into infinite loop
        if (rewriteContext.HttpContext.Request.Path.Value!.EndsWith(".ico", StringComparison.Ordinal))
            return;

        IRequestCultureFeature cultureFeature = rewriteContext.HttpContext.Features.Get<IRequestCultureFeature>();

        var actualCulture = cultureFeature.RequestCulture.Culture.Name;

        var requestedCulture = rewriteContext.HttpContext.GetRouteValue(_cultureRouteKey)?.ToString();

        if ($"c={actualCulture}|uic={actualCulture}"
         != rewriteContext.HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName])
            rewriteContext.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(actualCulture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

        //TODO: ensure to give precedence to cookie containing localization.
        // Either redirect here, or change culture model needs to be aware of more corner cases.
        if (!string.IsNullOrEmpty(requestedCulture)
         && (_cultureItems.Any(x => x.Name == requestedCulture)
          || string.Equals(requestedCulture, actualCulture, StringComparison.OrdinalIgnoreCase)))
            return;

        rewriteContext.HttpContext.GetRouteData().Values[_cultureRouteKey] = actualCulture;

        HttpResponse response = rewriteContext.HttpContext.Response;
        response.StatusCode   = StatusCodes.Status301MovedPermanently;
        rewriteContext.Result = RuleResult.EndResponse;

        // preserve query part parameters of the URL (?parameters) if there were any
        response.Headers[HeaderNames.Location] =
            _linkGenerator.GetPathByAction(
                rewriteContext.HttpContext,
                values: rewriteContext.HttpContext.GetRouteData().Values
            )
          + rewriteContext.HttpContext.Request.QueryString;
    }
}
