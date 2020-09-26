namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    using Aviant.DDD.Application.UseCases;

    public class ExportTodoListInput : UseCaseInput
    {
        public ExportTodoListInput(int listId) => ListId = listId;

        public int ListId { get; }
    }
}