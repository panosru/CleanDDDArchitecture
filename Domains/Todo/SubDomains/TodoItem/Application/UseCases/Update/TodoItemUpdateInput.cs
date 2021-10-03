namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using Aviant.DDD.Application.UseCases;

    public sealed class TodoItemUpdateInput : UseCaseInput
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

        internal int Id { get; }

        internal string Title { get; }

        internal bool Done { get; }
    }
}
