// ReSharper disable MemberCanBeInternal

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

#pragma warning disable 8618

public sealed class ExportTodosVm
{
    public string FileName { get; set; }

    public string ContentType { get; set; }

    public byte[] Content { get; set; }
}