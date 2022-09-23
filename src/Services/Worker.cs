using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GettingMessagesTelegram.Services;

public class Worker: BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHost _host;
    private readonly IChannelsService _channelsService;

    public Worker(ILogger<Worker> logger, IHost host, IChannelsService channelsService)
    {
        _logger = logger;
        _host = host;
        _channelsService = channelsService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _channelsService.WorkAsync(stoppingToken);
        await _host.StopAsync(stoppingToken);
        _logger.LogInformation("Application finished");
    }
}
