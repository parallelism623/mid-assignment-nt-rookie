

using ClosedXML.Excel;
using MIDASM.Application.Commons.Models.ImportExport;
using MIDASM.Application.Services.ImportExport;

namespace MIDASM.Infrastructure.ImportExport.Export;

public class ExportToExcel : IExportServices
{
    public ExportData<T> Export<T>(IEnumerable<T> dataExport)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add(nameof(T));

            var columns = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance 
                                                | System.Reflection.BindingFlags.Public 
                                                | System.Reflection.BindingFlags.DeclaredOnly)
                                                .Select(c => c.Name).ToList();

            int row = 1;
            for (int i = 0; i < columns.Count; i++)
            {
                worksheet.Cell(row, i + 1).Value = columns[i];
            }

            row += 1;
            var data = dataExport.Select(c =>
            {
                var dictionary = new Dictionary<string, object>();
                foreach (var it in typeof(T).GetProperties().Where(p => p.DeclaringType == typeof(T)))
                {
                    dictionary.Add(it.Name, it.GetValue(c) ?? default!);
                }
                return dictionary;
            }).ToList();

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < columns.Count; j++)
                {
                    worksheet.Cell(row, j + 1).Value = data[i][columns[j]]?.ToString();
                }
                row++;
            }
            using (var stream = new MemoryStream())
            {
                var exportModel = new ExportData<T>();
                workbook.SaveAs(stream);
                stream.Position = 0;

                exportModel.DataBytes = stream.ToArray();
                exportModel.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                exportModel.FileName = $"list-{nameof(T)}.xlsx";
                return exportModel;
            }
        }
    }
}
