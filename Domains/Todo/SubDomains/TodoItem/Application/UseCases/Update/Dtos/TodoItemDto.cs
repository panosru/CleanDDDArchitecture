namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update.Dtos
{
    using AutoMapper;
    using Aviant.DDD.Application.Mappings;
    using Aviant.DDD.Domain.TransferObjects;
    using Todo.Core.Entities;

    public class TodoItemDto : Dto, IMapFrom<TodoItemEntity>
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public string Title { get; set; }

        public bool IsCompleted { get; set; }

    #region IMapFrom<TodoItemEntity> Members

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TodoItemEntity, TodoItemDto>();
        }

    #endregion
    }
}