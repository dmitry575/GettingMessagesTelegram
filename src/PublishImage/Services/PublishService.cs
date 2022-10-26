using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PublishImage.Services;

public class PublishService : BackgroundService
{
    private const int Rows = 20;
    private readonly IMediaService _mediaService;
    private readonly IPostImages _postImages;
    private readonly ILogger<PublishService> _logger;

    public PublishService(IMediaService mediaService, IPostImages postImages, ILogger<PublishService> logger)
    {
        _mediaService = mediaService;
        _postImages = postImages;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        long id = -1;
        var count = 0;
        List<Media> photos;
        while ((photos = await _mediaService.GetPhotosNotSent(id, Rows, stoppingToken)) != null)
        {
            if (photos.Count <= 0)
            {
                _logger.LogInformation("no images for sending to site");
                break;
            }

            foreach (var photo in photos)
            {
                if (!File.Exists(photo.LocalPath))
                {
                    _logger.LogInformation($"image not exists: {photo.Id}, url: {photo.LocalPath}");
                    await _mediaService.Delete(photo.Id);
                    continue;
                }
                var result = await _postImages.SendAsync(photo);
                if (result.Success)
                {
                    _logger.LogInformation($"image sent to hosting: {photo.Id}, url: {result.Url}");

                    await _mediaService.UpdateSend(photo.Id, result.Url);
                    count++;
                }
                id = photo.Id;
            }

            _logger.LogInformation($"images sent to hosting: {count}");
        }
    }
}
