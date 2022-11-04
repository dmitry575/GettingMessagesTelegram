using GettingMessagesTelegram.Drivers.PostImage;
using GettingMessagesTelegram.Drivers.Translates;
using Microsoft.Extensions.Hosting;

namespace ImportTranslate.Services;

public class ImportService : BackgroundService
{
    private readonly IImport _importMediaService;
    
    public ImportService(IImport importMediaService)
    {
        _importMediaService = importMediaService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await _importMediaService.ImportAsync(stoppingToken);
    }
}
