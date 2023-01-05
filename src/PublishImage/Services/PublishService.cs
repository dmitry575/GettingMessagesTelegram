using GettingMessagesTelegram.Drivers.PostImage;
using Microsoft.Extensions.Hosting;

namespace PublishImage.Services;

public class PublishService : BackgroundService
{
    private readonly IPublishMediaService _publishMediaService;
    private readonly IHost _host;


    public PublishService(IPublishMediaService publishMediaService, IHost host)
    {
        _publishMediaService = publishMediaService;
        _host = host;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await _publishMediaService.ExecuteAsync(stoppingToken);
        await _host.StopAsync(stoppingToken);
    }
}
