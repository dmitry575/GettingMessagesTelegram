using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Drivers.PostImage;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
