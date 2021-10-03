// ReSharper disable MemberCanBeInternal

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    public sealed class ExportTodosVm
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}
