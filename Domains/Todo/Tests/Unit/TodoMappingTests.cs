using Aviant.Core.Configuration;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.Dtos;
using FluentAssertions;
using Xunit;
using CreatedTodoItemViewModel = CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create.TodoItemViewModel;
using UpdatedTodoItemViewModel = CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update.TodoItemViewModel;

namespace CleanDDDArchitecture.Domains.Todo.Tests.Unit;

public sealed class TodoMappingTests
{
    private static TodoItemEntity CompletedItem() => new()
    {
        Id          = 7,
        ListId      = 3,
        Title       = "Pears",
        IsCompleted = true,
        Priority    = PriorityLevel.High,
        Note        = "Conference pears"
    };

    [Fact]
    public void TodoItemDtoProjectionShouldMapEveryField()
    {
        var dto = TodoItemDto.Projection.Compile()(CompletedItem());

        dto.Id.Should().Be(7);
        dto.ListId.Should().Be(3);
        dto.Title.Should().Be("Pears");
        dto.Done.Should().BeTrue("Done reflects the entity's IsCompleted flag");
        dto.Priority.Should().Be((int)PriorityLevel.High);
        dto.Note.Should().Be("Conference pears");
    }

    [Fact]
    public void TodoItemDtoProjectionShouldKeepAMissingNoteNull()
    {
        var item = CompletedItem();
        item.Note = null;

        TodoItemDto.Projection.Compile()(item).Note.Should().BeNull();
    }

    [Fact]
    public void TodoListDtoProjectionShouldIncludeTheListItems()
    {
        var list = new TodoListEntity { Id = 3, Title = "Groceries" };
        ((List<TodoItemEntity>)list.Items).Add(CompletedItem());

        var dto = TodoListDto.Projection.Compile()(list);

        dto.Id.Should().Be(3);
        dto.Title.Should().Be("Groceries");
        dto.Items.Should().ContainSingle().Which.Done.Should().BeTrue();
    }

    [Fact]
    public void TodoItemRecordProjectionShouldExportTitleAndCompletion()
    {
        var record = TodoItemRecord.Projection.Compile()(CompletedItem());

        record.Title.Should().Be("Pears");
        record.Done.Should().BeTrue();
    }

    [Fact]
    public void ViewModelsShouldCopyTheirFieldsFromTheEntity()
    {
        var item = CompletedItem();

        var created = CreatedTodoItemViewModel.From(item);
        created.Id.Should().Be(7);
        created.ListId.Should().Be(3);
        created.Title.Should().Be("Pears");

        var updated = UpdatedTodoItemViewModel.From(item);
        updated.IsCompleted.Should().BeTrue();

        var list = CreatedTodoListViewModel.From(new TodoListEntity { Id = 3, Title = "Groceries" });
        list.Id.Should().Be(3);
        list.Title.Should().Be("Groceries");
    }
}
