

using ClosedXML.Excel;
using MIDASM.Application.Commons.Models.ImportExport;
using MIDASM.Application.Services.ImportExport;
using MIDASM.Contract.Helpers;
using Rookies.Contract.Exceptions;

namespace MIDASM.Infrastructure.ImportExport.Export.Excels;

public class ExportToExcel : IExportServices
{
    public ExportResponse Export<T>(ExportRequest<T> exportRequest)
    {
        using (var workbook = new XLWorkbook())
        {

            if(exportRequest.DataExport == null)
            {
                exportRequest.DataExport = new List<T>();
            }
            var columns = ReflectionHelper.GetDeclareProperties<T>();
            if(columns.Count == 0)
            {
                throw new BadRequestException("Can not export file beacause data invalid");
            }    
            int row = 1;
            var worksheet = workbook.Worksheets.Add("Export Data");

            var data = exportRequest.DataExport.Select(c =>
            {
                return c.GetDictionaryValueDeclareProperties();
            }).ToList();

            if (!string.IsNullOrEmpty(exportRequest.Title))
            {
                var r = worksheet.Range(row, 1, row, columns.Count);
                r.Merge()
                 .Style.Font.SetBold()
                 .Font.SetFontSize(16)
                 .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(row, 1).Value = exportRequest.Title;
                row++;
            }

           
            if (exportRequest.FromDate != null && exportRequest.ToDate != null)
            {
                var r = worksheet.Range(row, 1, row, columns.Count);
                r.Merge()
                 .Style.Font.SetItalic()
                 .Font.SetFontSize(12)
                 .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(row, 1).Value = $"From: {exportRequest.FromDate} - To: {exportRequest.ToDate}";
                row++;
            }

         
            for (int i = 0; i < columns.Count; i++)
            {
                worksheet.Cell(row, i + 1).Value = columns[i];
                worksheet.Cell(row, i + 1).Style.Font.SetBold();
            }

            int dataRowStart = row + 1;
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < columns.Count; j++)
                {
                    worksheet.Cell(dataRowStart + i, j + 1).Value = data[i][columns[j]]?.ToString() ?? "";
                }
            }
            worksheet.RangeUsed().Style.Alignment.WrapText = true;

            worksheet.Columns(1, columns.Count).AdjustToContents();

            foreach (var col in worksheet.Columns(1, columns.Count))
            {
                if (col.Width > 50)
                    col.Width = 50;
            }

            worksheet.Rows().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream, validate: false);
                stream.Position = 0;
                return new ExportResponse
                {
                    DataBytes = stream.ToArray(),
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = $"list-data.xlsx"
                };
            }
        }
    }
}
