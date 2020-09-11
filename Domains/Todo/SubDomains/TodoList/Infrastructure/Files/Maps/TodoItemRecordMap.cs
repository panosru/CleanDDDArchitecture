namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Files.Maps
{
    #region

    using System.Globalization;
    using Application.UseCases.Export;
    using CsvHelper.Configuration;

    #endregion

    public class TodoItemRecordMap : ClassMap<TodoItemRecord>
    {
        public TodoItemRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);

            Map(m => m.Done)
               .ConvertUsing(
                    c => c.Done
                        ? "Yes"
                        : "No");
        }
    }
}