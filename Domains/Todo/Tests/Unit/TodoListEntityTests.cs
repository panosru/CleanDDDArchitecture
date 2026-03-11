using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Todo.Tests.Unit;

public sealed class TodoListEntityTests
{
    [Fact]
    public async Task ValidateAsyncShouldRejectShortTitles()
    {
        var entity = new TodoListEntity { Title = "Tiny" };

        var isValid = await entity.ValidateAsync(TestContext.Current.CancellationToken);

        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateAsyncShouldAcceptTitlesLongerThanFiveCharacters()
    {
        var entity = new TodoListEntity { Title = "Groceries" };

        var isValid = await entity.ValidateAsync(TestContext.Current.CancellationToken);

        isValid.Should().BeTrue();
    }
}
