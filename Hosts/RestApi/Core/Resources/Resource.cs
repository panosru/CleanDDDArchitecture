using System.Globalization;
using System.Resources;

namespace CleanDDDArchitecture.Hosts.RestApi.Core.Resources;

/// <summary>
///  Resource Manager
/// </summary>
public static class Resource
{
    /// <summary>
    ///  Resource Manager
    /// </summary>
    private static readonly ResourceManager ResourceManager =
        new("CleanDDDArchitecture.Hosts.RestApi.Core.Resources.ProgramStrings", typeof(Resource).Assembly);
    
    /// <summary>
    ///  Get string from resource file
    /// </summary>
    /// <param name="key"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static string GetString(string key, CultureInfo culture) => ResourceManager.GetString(key, culture);
    
    /// <summary>
    ///   Get string from resource file
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetString(string key) => GetString(key, CultureInfo.CurrentCulture);
    
    // Strings:
    
    public static string HostTerminatedUnexpectedly => GetString(nameof(HostTerminatedUnexpectedly));
}
