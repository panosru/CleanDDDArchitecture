using System.Collections.Immutable;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace CleanDDDArchitecture.Hosts.WebApp.Core.Localisation;

public static class Cultures
{
    public static readonly ImmutableList<CultureInfo> SupportedCultures = ImmutableList.Create(
        new CultureInfo("en"),
        new CultureInfo("el"));

    public static readonly RequestCulture DefaultRequestCulture = new(SupportedCultures[0].Name);
}
