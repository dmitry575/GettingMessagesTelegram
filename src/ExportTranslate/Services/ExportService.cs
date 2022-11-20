using GettingMessagesTelegram.Drivers.Translates;
using Microsoft.Extensions.Hosting;

namespace ExportTranslate.Services;

public class ExportService : BackgroundService
{
    private readonly IExport _exportMediaService;
    
    public ExportService(IExport exportMediaService)
    {
        _exportMediaService = exportMediaService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _exportMediaService.ExportAsync(stoppingToken);
    }
}
