namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Files.Maps;

using System.Globalization;
using Application.UseCases.Export;
using CsvHelper.Configuration;

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