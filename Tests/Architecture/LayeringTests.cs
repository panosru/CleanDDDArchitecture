using System.Reflection;
using Aviant.Core.EventSourcing.Aggregates;
using Aviant.Core.EventSourcing.DomainEvents;
using AwesomeAssertions;
using NetArchTest.Rules;
using Xunit;

namespace CleanDDDArchitecture.Tests.Architecture;

/// <summary>
///     The dependency rules of the architecture, enforced. Each rule is written against the
///     naming convention, so a new domain or layer is covered without editing this file.
/// </summary>
public sealed class LayeringTests
{
    public static TheoryData<string> CoreAssemblies => Names("Core");

    public static TheoryData<string> ApplicationAssemblies => Names("Application");

    public static TheoryData<string> AllAssemblies => new(DomainModel.Assemblies.Select(a => a.GetName().Name!));

    [Fact]
    public void TheRulesSeeTheWholeModel() =>
        DomainModel.Assemblies.Select(DomainModel.LayerOf).Distinct()
           .Should().Contain(["Core", "Application", "Infrastructure", "CrossCutting", "Hosts"]);

    [Theory]
    [MemberData(nameof(CoreAssemblies))]
    public void CoreDependsOnNoOuterLayer(string assembly)
    {
        var result = Types.InAssembly(Load(assembly))
           .ShouldNot()
           .HaveDependencyOnAny(DomainModel.NamespacesOf("Application", "Infrastructure", "CrossCutting", "Hosts"))
           .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty($"{assembly} is the domain model and must not know about outer layers");
    }

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    public void ApplicationDependsOnNeitherInfrastructureNorHosts(string assembly)
    {
        var result = Types.InAssembly(Load(assembly))
           .ShouldNot()
           .HaveDependencyOnAny(DomainModel.NamespacesOf("Infrastructure", "Hosts"))
           .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty($"{assembly} must depend on abstractions, not on infrastructure");
    }

    [Theory]
    [MemberData(nameof(AllAssemblies))]
    public void DomainsDependOnlyOnThemselvesAndShared(string assembly)
    {
        var domain = DomainModel.DomainOf(assembly);
        string[] otherDomains = DomainModel.Assemblies
           .Select(a => a.GetName().Name!)
           .Where(name => DomainModel.DomainOf(name) is var other && other != domain && other != "Shared")
           .ToArray();

        var result = Types.InAssembly(Load(assembly))
           .ShouldNot()
           .HaveDependencyOnAny(otherDomains)
           .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty(
            $"bounded contexts talk through Shared contracts and integration events, not by referencing each other ({assembly})");
    }

    [Fact]
    public void AggregatesAndDomainEventsLiveInTheCoreLayer()
    {
        var misplaced = DomainModel.Assemblies
           .Where(assembly => DomainModel.LayerOf(assembly) != "Core")
           .SelectMany(DomainModel.Types)
           .Where(type => DerivesFrom(type, typeof(Aggregate<,>)) || DerivesFrom(type, typeof(DomainEvent<,>)))
           .Select(type => type.FullName)
           .ToList();

        misplaced.Should().BeEmpty("aggregates and their domain events are the domain model, which belongs in Core");
    }

    private static bool DerivesFrom(Type type, Type openGeneric)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
            if (current.IsGenericType && current.GetGenericTypeDefinition() == openGeneric)
                return true;

        return false;
    }

    private static Assembly Load(string name) => DomainModel.Assemblies.Single(a => a.GetName().Name == name);

    private static TheoryData<string> Names(string layer) =>
        new(DomainModel.InLayer(layer).Select(a => a.GetName().Name!));
}
