namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using Aviant.DDD.Application.UseCases;

    public sealed class UpdateTodoListInput : UseCaseInput
    {
        public UpdateTodoListInput(int id, string title)
        {
            Id    = id;
            Title = title;
        }

        internal int Id { get; }

        internal string Title { get; }
    }
}