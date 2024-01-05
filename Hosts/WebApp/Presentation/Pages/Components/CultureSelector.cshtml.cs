namespace CleanDDDArchitecture.Hosts.WebApp.Presentation.Pages.Components;

using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class CultureSelector : ViewComponent
{
    /// <summary>
    /// 
    /// </summary>
    private string _returnUrl;

    /// <summary>
    /// 
    /// </summary>
    private string _currentCulture;

    public CultureSelector(IHttpContextAccessor httpContextAccessor)
    {
        var request = httpContextAccessor.HttpContext?.Request;

        _returnUrl = string.IsNullOrEmpty(request?.Path)
            ? "/"
            : $"{request.Path}{request.QueryString}";

        _currentCulture = CultureInfo.CurrentCulture.Name;

        string newCulture = _currentCulture switch
        {
            "en" => "/el",
            "el" => "/en",
            _    => "/unsupported"
            // _    => $"/{Startup.DefaultRequestCulture.Culture.ToString().ToLower()}"
        };

        _returnUrl = _returnUrl
           .Replace($"/{_currentCulture}", newCulture, StringComparison.OrdinalIgnoreCase)
           .ToLower();
    }

    public IViewComponentResult Invoke(string name) =>
        View(
            "/Pages/Components/CultureSelector.cshtml",
            new Dictionary<string, string>
            {
                { "ReturnUrl", _returnUrl },
                { "CurrentCulture", _currentCulture }
            });
}
