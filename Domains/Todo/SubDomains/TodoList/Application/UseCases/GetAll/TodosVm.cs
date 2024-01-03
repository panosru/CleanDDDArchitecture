// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeInternal

using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.Dtos;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll;

internal sealed class TodosVm
{
    public IList<PriorityLevelDto> PriorityLevels { get; set; }

    public IList<TodoListDto> Lists { get; set; }
}
