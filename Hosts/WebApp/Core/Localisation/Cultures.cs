namespace CleanDDDArchitecture.Hosts.WebApp.Core.Localisation;

using System.Collections.Immutable;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

public static class Cultures
{
    public static readonly ImmutableList<CultureInfo> SupportedCultures = ImmutableList.Create(
        new CultureInfo("en"),
        new CultureInfo("el"));

    public static readonly RequestCulture DefaultRequestCulture = new(SupportedCultures[0].Name);
}