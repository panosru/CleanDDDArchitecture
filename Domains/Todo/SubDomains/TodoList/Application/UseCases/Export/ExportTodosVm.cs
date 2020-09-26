namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    public class ExportTodosVm
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}