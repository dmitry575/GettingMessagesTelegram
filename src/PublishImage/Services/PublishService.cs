using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PublishImage.Services;

public class PublishService : BackgroundService
{
    private const int Rows = 20;
    private readonly IMediaService _mediaService;
    private readonly ILogger<PublishService> _logger;

    public PublishService(IMediaService mediaService, ILogger<PublishService> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        long id = -1;
        List<Media> photos;
        while ((photos = await _mediaService.GetPhotosNotSent(id, Rows, stoppingToken)) != null)
        {
            if (photos.Count <= 0)
            {
                _logger.LogInformation("no images for sending to site");
                break;
            }
            
            
        }
    }
}
