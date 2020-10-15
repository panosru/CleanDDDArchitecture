namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using Aviant.DDD.Application.UseCases;

    /// <inheritdoc />
    /// <summary>
    ///     Create Todo List Input Data Object
    /// </summary>
    public sealed class CreateTodoListInput : UseCaseInput
    {
        /// <summary>
        ///     Create Todo List Input Constructor
        /// </summary>
        /// <param name="title">The title of the todo list</param>
        public CreateTodoListInput(string title) => Title = title;

        internal string Title { get; }
    }
}