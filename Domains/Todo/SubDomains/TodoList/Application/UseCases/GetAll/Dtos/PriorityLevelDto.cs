// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeInternal

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.Dtos;

internal struct PriorityLevelDto
{
    public int Value { get; set; }

    public string Name { get; set; }
}