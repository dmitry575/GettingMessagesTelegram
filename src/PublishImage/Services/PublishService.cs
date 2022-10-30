using GettingMessagesTelegram.Drivers.PostImage;
using Microsoft.Extensions.Hosting;

namespace PublishImage.Services;

public class PublishService : BackgroundService
{
    private readonly IPublishMediaService _publishMediaService;
    
    public PublishService(IPublishMediaService publishMediaService)
    {
        _publishMediaService = publishMediaService;
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await _publishMediaService.ExecuteAsync(stoppingToken);
    }
}
