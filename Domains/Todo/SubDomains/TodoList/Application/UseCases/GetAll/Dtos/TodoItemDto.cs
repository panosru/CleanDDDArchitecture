#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.Dtos;

using AutoMapper;
using Aviant.Application.Mappings;
using Todo.Core.Entities;

public sealed class TodoItemDto : IMapFrom<TodoItemEntity>
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public string Title { get; set; }

    public bool Done { get; set; }

    public int Priority { get; set; }

    public string Note { get; set; }

    #region IMapFrom<TodoItemEntity> Members

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TodoItemEntity, TodoItemDto>()
           .ForMember(
                d =>
                    d.Priority,
                opt =>
                    opt.MapFrom(s => (int)s.Priority));
    }

    #endregion
}
