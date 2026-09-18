using System.Reflection;

namespace CleanDDDArchitecture.Tests.Architecture;

/// <summary>
///     Loads every production assembly under <c>Domains/</c> and classifies it by the naming
///     convention the repository uses: <c>CleanDDDArchitecture.Domains.{Domain}[.SubDomains.{Sub}].{Layer}</c>.
/// </summary>
internal static class DomainModel
{
    private const string Prefix = "CleanDDDArchitecture.Domains.";

    public static readonly IReadOnlyList<Assembly> Assemblies = LoadDomainAssemblies();

    /// <summary>The top-level domain of an assembly or namespace: Account, Todo, Weather, Shared.</summary>
    public static string DomainOf(string name) => name[Prefix.Length..].Split('.')[0];

    /// <summary>The layer of an assembly: Core, Application, Infrastructure, CrossCutting or Hosts.</summary>
    public static string LayerOf(Assembly assembly)
    {
        string[] parts = assembly.GetName().Name![Prefix.Length..].Split('.');

        return parts.Contains("Hosts") ? "Hosts" : parts[^1];
    }

    public static IEnumerable<Assembly> InLayer(string layer) => Assemblies.Where(a => LayerOf(a) == layer);

    /// <summary>Namespace prefixes of every assembly in the given layers.</summary>
    public static string[] NamespacesOf(params string[] layers) =>
        Assemblies.Where(a => layers.Contains(LayerOf(a))).Select(a => a.GetName().Name!).ToArray();

    public static IEnumerable<Type> Types(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }

    private static List<Assembly> LoadDomainAssemblies()
    {
        // The test project references every Domains/ project, so they sit next to this assembly.
        var directory = Path.GetDirectoryName(typeof(DomainModel).Assembly.Location)!;

        return Directory.GetFiles(directory, Prefix + "*.dll")
           .Select(path => Assembly.Load(Path.GetFileNameWithoutExtension(path)))
           .Where(assembly => !assembly.GetName().Name!.Contains(".Tests", StringComparison.Ordinal))
           .OrderBy(assembly => assembly.GetName().Name, StringComparer.Ordinal)
           .ToList();
    }
}
