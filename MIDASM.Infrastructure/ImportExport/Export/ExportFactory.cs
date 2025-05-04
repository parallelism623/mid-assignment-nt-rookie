
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Commons.Models.ImportExport;
using MIDASM.Application.Services.ImportExport;
using Rookies.Contract.Exceptions;

namespace MIDASM.Infrastructure.ImportExport.Export;

public class ExportFactory : IExportFactory
{
    private readonly IServiceProvider _serviceProvider;
    public ExportFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public ExportResponse ExportFile<T>(string fileType, ExportRequest<T> exportRequest)
    {
        var exportService = _serviceProvider.GetRequiredKeyedService<IExportServices>(fileType);
        if(exportService == null)
        {
            throw new BadRequestException("File export type does not current support!");
        }

        return exportService.Export(exportRequest); 
    }
}
