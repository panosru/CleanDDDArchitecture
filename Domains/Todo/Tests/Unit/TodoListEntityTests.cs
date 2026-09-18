using Aviant.Core.Exceptions;
using AwesomeAssertions;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using Xunit;

namespace CleanDDDArchitecture.Domains.Todo.Tests.Unit;

public sealed class TodoListEntityTests
{
    [Fact]
    public void CreateTrimsTheTitle() =>
        TodoListEntity.Create("  Groceries ").Title.Should().Be("Groceries");

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRefusesABlankTitle(string title)
    {
        var act = () => TodoListEntity.Create(title);

        act.Should().Throw<DomainRuleException>();
    }

    [Fact]
    public void RenameRefusesATitleLongerThanTheLimit()
    {
        var list = TodoListEntity.Create("Groceries");

        var act = () => list.Rename(new string('x', TodoListEntity.TitleMaxLength + 1));

        act.Should().Throw<DomainRuleException>();
        list.Title.Should().Be("Groceries");
    }

    [Fact]
    public async Task AListThatObeysItsRulesIsValidForPersistence() =>
        (await TodoListEntity.Create("Tiny").ValidateAsync(TestContext.Current.CancellationToken)).Should().BeTrue();
}
