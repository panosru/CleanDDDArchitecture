namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create.Dtos
{
    #region

    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    #endregion

    public class TodoListDto : IMapFrom<TodoListEntity>
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}