using CleanDDDArchitecture.Hosts.WebApp.Core.Localisation;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;

namespace CleanDDDArchitecture.Hosts.WebApp.Presentation.ServiceExtensions;

/// <summary>
///  Add Razor Pages and Localization services
/// </summary>
public static class PagesAndLocale
{
    /// <summary>
    ///   Add Razor Pages and Localization services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddPagesAndLocaleServices(this IServiceCollection services)
    {
        services.AddRazorPages(
                options => options.Conventions.Add(new CultureTemplatePageRouteModelConvention()))
            .AddRazorRuntimeCompilation()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(
                options => options.DataAnnotationLocalizerProvider = (_, factory)
                    => factory.Create(typeof(SharedResource)));

        services.AddLocalization(options => options.ResourcesPath = "Resources");
        
        services.Configure<RequestLocalizationOptions>(
            options =>
            {
                options.DefaultRequestCulture = Cultures.DefaultRequestCulture;
                options.SupportedCultures     = Cultures.SupportedCultures;
                options.SupportedUICultures   = Cultures.SupportedCultures;

                options.RequestCultureProviders.Insert(
                    0,
                    new RouteDataRequestCultureProvider
                    {
                        RouteDataStringKey   = "culture",
                        UIRouteDataStringKey = "culture",
                        Options              = options
                    });
            });

        services.AddRouting(
            options =>
            {
                options.LowercaseUrls         = true;
                options.LowercaseQueryStrings = true;
                // options.AppendTrailingSlash   = true;
            });

        services.AddTransient<RedirectUnsupportedCulturesRule>();
        
        return services;
    }
}
