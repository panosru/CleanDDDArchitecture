using CleanDDDArchitecture.Hosts.WebApp.Core.Localisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CleanDDDArchitecture.Hosts.WebApp.Core.TagHelpers;

[HtmlTargetElement("a", Attributes = ActionAttributeName)]
[HtmlTargetElement("a", Attributes = ControllerAttributeName)]
[HtmlTargetElement("a", Attributes = AreaAttributeName)]
[HtmlTargetElement("a", Attributes = PageAttributeName)]
[HtmlTargetElement("a", Attributes = PageHandlerAttributeName)]
[HtmlTargetElement("a", Attributes = FragmentAttributeName)]
[HtmlTargetElement("a", Attributes = HostAttributeName)]
[HtmlTargetElement("a", Attributes = ProtocolAttributeName)]
[HtmlTargetElement("a", Attributes = RouteAttributeName)]
[HtmlTargetElement("a", Attributes = RouteValuesDictionaryName)]
[HtmlTargetElement("a", Attributes = RouteValuesPrefix + "*")]
public class CultureAnchor : AnchorTagHelper
{
    public CultureAnchor(IHttpContextAccessor contextAccessor, IHtmlGenerator generator)
        :
        base(generator) => _contextAccessor = contextAccessor;

    /// <summary>
    /// </summary>
    private const string ActionAttributeName = "asp-action";

    /// <summary>
    /// </summary>
    private const string ControllerAttributeName = "asp-controller";

    /// <summary>
    /// </summary>
    private const string AreaAttributeName = "asp-area";

    /// <summary>
    /// </summary>
    private const string PageAttributeName = "asp-page";

    /// <summary>
    /// </summary>
    private const string PageHandlerAttributeName = "asp-page-handler";

    /// <summary>
    /// </summary>
    private const string FragmentAttributeName = "asp-fragment";

    /// <summary>
    /// </summary>
    private const string HostAttributeName = "asp-host";

    /// <summary>
    /// </summary>
    private const string ProtocolAttributeName = "asp-protocol";

    /// <summary>
    /// </summary>
    private const string RouteAttributeName = "asp-route";

    /// <summary>
    /// </summary>
    private const string RouteValuesDictionaryName = "asp-all-route-data";

    /// <summary>
    /// </summary>
    private const string RouteValuesPrefix = "asp-route-";

    /// <summary>
    /// </summary>
    private const string Href = "href";

    /// <summary>
    /// </summary>
    private readonly IHttpContextAccessor _contextAccessor;

    /// <summary>
    /// </summary>
    private readonly string _defaultRequestCulture = Cultures.DefaultRequestCulture.Culture.ToString().ToLower();

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        #pragma warning disable 8600
        #pragma warning disable 8602
        RouteValues["culture"] = (string)_contextAccessor.HttpContext.Request.RouteValues["culture"]
                              ?? _defaultRequestCulture;
        #pragma warning restore 8602
        #pragma warning restore 8600

        base.Process(context, output);
    }
}