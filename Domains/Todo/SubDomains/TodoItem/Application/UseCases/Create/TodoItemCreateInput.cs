namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create
{
    using Aviant.DDD.Application.UseCases;

    public class TodoItemCreateInput : IUseCaseInput
    {
        public TodoItemCreateInput(int listId, string title)
        {
            ListId = listId;
            Title  = title;
        }

        public int ListId { get; }

        public string Title { get; }
    }
}