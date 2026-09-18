using System.Reflection;
using System.Text.RegularExpressions;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Xunit;

namespace CleanDDDArchitecture.Tests.Architecture;

/// <summary>
///     Rules for controller actions in the domains' presentation layers.
/// </summary>
public sealed partial class HttpBindingTests
{
    /// <summary>
    ///     A route like <c>updatedetails/{id}</c> with a <c>[FromQuery] int id</c> parameter never
    ///     binds the path value: the action sees 0, and the duplicate "id" parameter also breaks
    ///     OpenAPI document generation for the entire API.
    /// </summary>
    [Fact]
    public void RouteValuesAreNotBoundFromTheQueryString()
    {
        var offenders = DomainModel.Assemblies
           .Where(assembly => DomainModel.LayerOf(assembly) == "Hosts")
           .SelectMany(DomainModel.Types)
           .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
           .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
           .SelectMany(
                action => action.GetCustomAttributes<HttpMethodAttribute>()
                   .Where(http => http.Template is not null)
                   .SelectMany(
                        http => action.GetParameters()
                           .Where(parameter => parameter.GetCustomAttribute<FromQueryAttribute>() is not null)
                           .Where(parameter => RouteTokens(http.Template!).Contains(parameter.Name!, StringComparer.OrdinalIgnoreCase))
                           .Select(parameter => $"{action.DeclaringType!.FullName}.{action.Name}({parameter.Name}) on \"{http.Template}\"")))
           .ToList();

        offenders.Should().BeEmpty("a value that is part of the route must be bound with [FromRoute]");
    }

    private static IEnumerable<string> RouteTokens(string template) =>
        RouteToken().Matches(template).Select(match => match.Groups["name"].Value);

    [GeneratedRegex(@"\{\*{0,2}(?<name>[A-Za-z_][A-Za-z0-9_]*)")]
    private static partial Regex RouteToken();
}
