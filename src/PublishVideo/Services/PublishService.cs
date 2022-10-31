using GettingMessagesTelegram.Drivers.PostImage;
using Microsoft.Extensions.Hosting;

namespace PublishVideo.Services;

public class PublishVideoService : BackgroundService
{
    private readonly IPublishMediaService _publishMediaService;
    
    public PublishVideoService(IPublishMediaService publishMediaService)
    {
        _publishMediaService = publishMediaService;
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await _publishMediaService.ExecuteAsync(stoppingToken);
    }
}
