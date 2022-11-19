using GettingMessagesTelegram.Drivers.PostImage.Impl;
using GettingMessagesTelegram.Helpers;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Logging;

namespace GettingMessagesTelegram.Drivers.Youtube.Impl
{
    public class PublishVideoService : IPublishVideoService
    {
        private const string Title = "War in Ukrain 2022";

        /// <summary>
        /// Url to youtube
        /// </summary>
        private const string YoutubeUrl = "https://youtu.be/";

        private const int Rows = 20;
        private readonly IMediaService _mediaService;
        private readonly IYouTubeUploader _uploader;
        private readonly IMessageService _messageService;
        private readonly ILogger<PublishMediaService> _logger;

        public PublishVideoService(IMediaService mediaService, IYouTubeUploader uploader,
            IMessageService messageService,
            ILogger<PublishMediaService> logger)
        {
            _mediaService = mediaService;
            _uploader = uploader;
            _messageService = messageService;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            long id = -1;
            var count = 0;
            while (await _mediaService.GetVideosNotSent(id, Rows, stoppingToken) is { } videos)
            {
                if (videos.Count <= 0)
                {
                    _logger.LogInformation("no videos for sending to hosting");
                    break;
                }

                foreach (var video in videos)
                {
                    if (!File.Exists(video.LocalPath))
                    {
                        _logger.LogInformation($"video not exists: {video.Id}, url: {video.LocalPath}");
                        await _mediaService.Delete(video.Id);
                        continue;
                    }

                    var message = await _messageService.GetById(video.MessageId);
                    var title = Title;
                    if (message != null)
                    {
                        title = WordHelper.GetSplitByWord(message.Content, 100);
                    }


                    var result = await _uploader.UploadAsync(title, message?.Content, video.LocalPath, stoppingToken);
                    if (result.Success)
                    {
                        _logger.LogInformation($"video sent to hosting: {video.Id}, url: {result.Url}");

                        await _mediaService.UpdateSend(video.Id, result.Url);
                        count++;
                    }
                    id = video.Id;
                }

                _logger.LogInformation($"videos sent to hosting: {count}");
            }
        }
    }
}
