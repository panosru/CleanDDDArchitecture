namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    using Aviant.DDD.Application.UseCases;

    public class ExportTodoListInput : IUseCaseInput
    {
        public ExportTodoListInput(int listId) => ListId = listId;

        public int ListId { get; }
    }
}