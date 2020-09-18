namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using Aviant.DDD.Application.UseCases;

    public class TodoItemUpdateInput : IUseCaseInput
    {
        public TodoItemUpdateInput(
            int    id,
            string title,
            bool   done)
        {
            Id    = id;
            Title = title;
            Done  = done;
        }

        public int Id { get; }

        public string Title { get; }

        public bool Done { get; }
    }
}