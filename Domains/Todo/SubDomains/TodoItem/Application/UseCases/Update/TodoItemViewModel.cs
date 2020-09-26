namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using AutoMapper;
    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    public class TodoItemViewModel : IMapFrom<TodoItemEntity>
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
}