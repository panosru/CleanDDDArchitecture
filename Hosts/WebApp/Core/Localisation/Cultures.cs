namespace CleanDDDArchitecture.Hosts.WebApp.Core.Localisation
{
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.AspNetCore.Localization;

    public static class Cultures
    {
        public static readonly List<CultureInfo> SupportedCultures = new()
        {
            new CultureInfo("en"),
            new CultureInfo("el")
        };

        public static readonly RequestCulture DefaultRequestCulture = new(SupportedCultures[0].Name);
    }
}