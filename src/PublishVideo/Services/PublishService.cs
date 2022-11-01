using GettingMessagesTelegram.Drivers.Youtube;
using Microsoft.Extensions.Hosting;

namespace PublishVideo.Services;

public class PublishService : BackgroundService
{
    private readonly IPublishVideoService _publishVideoService;

    public PublishService(IPublishVideoService publishVideoService)
    {
        _publishVideoService = publishVideoService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _publishVideoService.ExecuteAsync(stoppingToken);
    }
}
