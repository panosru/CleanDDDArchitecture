using System.Globalization;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;
using CsvHelper.Configuration;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Files.Maps;

public sealed class TodoItemRecordMap : ClassMap<TodoItemRecord>
{
    public TodoItemRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(m => m.Done)
           .Convert(
                args => args.Value.Done
                    ? "Yes"
                    : "No");
    }
}