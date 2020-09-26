namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using Aviant.DDD.Application.UseCases;

    public class UpdateTodoListInput : UseCaseInput
    {
        public UpdateTodoListInput(int id, string title)
        {
            Id    = id;
            Title = title;
        }

        public int Id { get; }

        public string Title { get; }
    }
}