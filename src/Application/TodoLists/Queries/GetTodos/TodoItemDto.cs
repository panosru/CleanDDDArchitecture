namespace CleanDDDArchitecture.Application.TodoLists.Queries.GetTodos
{
    using AutoMapper;
    using Aviant.DDD.Application.Mappings;
    using Domain.Entities;

    public class TodoItemDto : IMapFrom<TodoItemEntity>
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
                            opt.MapFrom(s => (int) s.Priority));
        }

    #endregion
    }
}