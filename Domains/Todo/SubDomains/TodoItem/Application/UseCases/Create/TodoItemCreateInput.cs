namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create
{
    using Aviant.DDD.Application.UseCases;

    public sealed class TodoItemCreateInput : UseCaseInput
    {
        public TodoItemCreateInput(int listId, string title)
        {
            ListId = listId;
            Title  = title;
        }

        internal int ListId { get; }

        internal string Title { get; }
    }
}