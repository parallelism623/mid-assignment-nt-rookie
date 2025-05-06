
using MIDASM.Application.Commons.Models.ImportExport;
using MIDASM.Contract.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MIDASM.Infrastructure.ImportExport.Export;

public class ExportToPdfBasicDocs<T> : IDocument
{
    private readonly ExportRequest<T> _request;

    public ExportToPdfBasicDocs(ExportRequest<T> request)
    {
        _request = request;
    }
    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);
                page.Header().Height(100).Background(Colors.Grey.Lighten1).Element(ComposeHeader);
                page.Content().Background(Colors.Grey.Lighten3).Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }


    void ComposeHeader(IContainer container)
    {
        container
        .PaddingBottom(10)
        .AlignCenter()
        .Column(column =>
        {
            column.Spacing(5); 

            column.Item().Text($"📘 {_request.Title}")
                .FontSize(18)
                .AlignCenter()
                .Bold();

            column.Item().Text($"From {_request.FromDate:dd/MM/yyyy} to {_request.ToDate:dd/MM/yyyy}")
                .FontSize(12)
                .Italic()
                .AlignCenter()
                .FontColor(Colors.Grey.Darken2);
        });
    }
    void ComposeContent(IContainer container)
    {
        container.Element(ComposeTable);
    }
    static void ComposeFooter(IContainer container)
    {

        container.AlignCenter().Text(x =>
        {
            x.CurrentPageNumber();
            x.Span(" / ");
            x.TotalPages();
        });
    }


    void ComposeTable(IContainer container)
    {
        if (_request == null)
        {
            throw new ArgumentException("Export to PDF failure beacause no data provider");
        }
        var columnsOfExportFile = ReflectionHelper.GetDeclareProperties<T>();
        var dataOfExportFile = _request.DataExport.Select(c =>
        {
            return c.GetDictionaryValueDeclareProperties();
        }).ToList();
        container.Table(table =>
        {

            table.ColumnsDefinition(columns =>
            {
                foreach (var column in columnsOfExportFile)
                    columns.RelativeColumn();
            });

            table.Header(header =>
            {
                foreach (var column in columnsOfExportFile)
                    header.Cell().Element(CellStyle).AlignCenter().Text(column).FontSize(10);

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderRight(1).BorderColor(Colors.Black);
                }
            });

            foreach (var item in dataOfExportFile)
            {
                foreach (var column in columnsOfExportFile)
                    table.Cell().Element(CellStyle).AlignCenter().Text(item[column]?.ToString() ?? string.Empty).FontSize(10);

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderRight(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            }
        });
    }
}
