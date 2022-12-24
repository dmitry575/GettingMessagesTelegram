using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GettingMessagesTelegram.Services;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHost _host;
    private readonly IReceiveService _receiveService;

    public Worker(ILogger<Worker> logger, IHost host, IReceiveService receiveService)
    {
        _logger = logger;
        _host = host;
        _receiveService = receiveService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _receiveService.WorkAsync(stoppingToken);

        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker finished, stop token");
            return;
        }
        _receiveService.SubscribeToEvents();

        _logger.LogInformation("Listener of event started");

        WaitHandle.WaitAny(new WaitHandle[] { stoppingToken.WaitHandle });
        await _host.StopAsync(stoppingToken);
        _logger.LogInformation("Worker finished");
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _receiveService?.Dispose();
        return base.StopAsync(cancellationToken);
    }
}
