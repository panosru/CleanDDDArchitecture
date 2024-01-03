using AutoMapper;
using Aviant.Application.Mappings;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

public sealed class TodoItemViewModel : IMapFrom<TodoItemEntity>
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public string Title { get; set; }

    public bool IsCompleted { get; set; }

    #region IMapFrom<TodoItemEntity> Members

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TodoItemEntity, TodoItemViewModel>();
    }

    #endregion
}
