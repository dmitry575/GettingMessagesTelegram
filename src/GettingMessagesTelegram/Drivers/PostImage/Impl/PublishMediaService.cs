using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Logging;

namespace GettingMessagesTelegram.Drivers.PostImage.Impl;

public class PublishMediaService : IPublishMediaService
{
    private const int Rows = 20;
    private readonly IMediaService _mediaService;
    private readonly IPostImages _postImages;
    private readonly ILogger<PublishMediaService> _logger;

    public PublishMediaService(IMediaService mediaService, IPostImages postImages, ILogger<PublishMediaService> logger)
    {
        _mediaService = mediaService;
        _postImages = postImages;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        long id = -1;
        var count = 0;
        List<Data.Media> photos;
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
