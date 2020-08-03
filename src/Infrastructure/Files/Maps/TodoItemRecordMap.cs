namespace CleanArchitecture.Infrastructure.Files.Maps
{
    using System.Globalization;
    using Application.TodoLists.Queries.ExportTodos;
    using CsvHelper.Configuration;

    public class TodoItemRecordMap : ClassMap<TodoItemRecord>
    {
        public TodoItemRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Done).ConvertUsing(c => c.Done ? "Yes" : "No");
        }
    }
}