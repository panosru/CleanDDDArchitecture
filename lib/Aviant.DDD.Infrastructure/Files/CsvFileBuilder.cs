namespace Aviant.DDD.Infrastructure.Files
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Application;
    using CsvHelper;
    using CsvHelper.Configuration;

    public class CsvFileBuilder<TRecrod, TMap> : ICsvFileBuilder<TRecrod>
        where TRecrod : class
        where TMap : ClassMap<TRecrod>
    {
        public byte[] BuildTodoItemsFile(IEnumerable<TRecrod> records)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

                csvWriter.Configuration.RegisterClassMap<TMap>();
                csvWriter.WriteRecords(records);
            }

            return memoryStream.ToArray();
        }
    }
}