using Aviant.Core.Configuration;
using Aviant.Core.Exceptions;
using AwesomeAssertions;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using Xunit;

namespace CleanDDDArchitecture.Domains.Todo.Tests.Unit;

public sealed class TodoItemEntityTests
{
    [Fact]
    public void CreateStartsActiveIncompleteAndMediumPriority()
    {
        var item = TodoItemEntity.Create(listId: 3, title: "  Buy pears  ");

        item.ListId.Should().Be(3);
        item.Title.Should().Be("Buy pears");
        item.IsCompleted.Should().BeFalse();
        item.Priority.Should().Be(PriorityLevel.Medium);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRefusesABlankTitle(string title)
    {
        var act = () => TodoItemEntity.Create(3, title);

        act.Should().Throw<DomainRuleException>().WithMessage("*title*");
    }

    [Fact]
    public void CreateRefusesATitleLongerThanTheLimit()
    {
        var act = () => TodoItemEntity.Create(3, new string('x', TodoItemEntity.TitleMaxLength + 1));

        act.Should().Throw<DomainRuleException>();
    }

    [Fact]
    public void CompleteAndReopenToggleCompletion()
    {
        var item = TodoItemEntity.Create(3, "Buy pears");

        item.Complete();
        item.IsCompleted.Should().BeTrue();

        item.Reopen();
        item.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void RenameAppliesTheSameTitleRules()
    {
        var item = TodoItemEntity.Create(3, "Buy pears");

        item.Rename("Buy apples");
        item.Title.Should().Be("Buy apples");

        var act = () => item.Rename(" ");
        act.Should().Throw<DomainRuleException>();
        item.Title.Should().Be("Buy apples");
    }

    [Fact]
    public void DetailsCanBeChanged()
    {
        var item = TodoItemEntity.Create(3, "Buy pears");

        item.MoveTo(4);
        item.SetPriority(PriorityLevel.High);
        item.SetNote("Conference pears");

        item.ListId.Should().Be(4);
        item.Priority.Should().Be(PriorityLevel.High);
        item.Note.Should().Be("Conference pears");
    }

    [Fact]
    public void AnEmptyNoteIsStoredAsNoNote()
    {
        var item = TodoItemEntity.Create(3, "Buy pears");

        item.SetNote("  ");

        item.Note.Should().BeNull();
    }
}
