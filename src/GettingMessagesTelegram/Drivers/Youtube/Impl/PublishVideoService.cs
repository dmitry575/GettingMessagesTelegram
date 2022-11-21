using System.Text;
using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Drivers.PostImage.Impl;
using GettingMessagesTelegram.Helpers;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Logging;

namespace GettingMessagesTelegram.Drivers.Youtube.Impl
{
    public class PublishVideoService : IPublishVideoService
    {
        /// <summary>
        /// Default title of videos
        /// </summary>
        private const string DefaultTitle = "War in Ukrain 2022";

        /// <summary>
        /// Default language fro title of video
        /// </summary>
        private const string DefaultLanguage = "en";

        private const int Rows = 20;
        private readonly IMediaService _mediaService;
        private readonly IYouTubeUploader _uploader;
        private readonly ILogger<PublishMediaService> _logger;

        public PublishVideoService(IMediaService mediaService, IYouTubeUploader uploader,
            ILogger<PublishMediaService> logger)
        {
            _mediaService = mediaService;
            _uploader = uploader;
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

                    var title = DefaultTitle;
                    var description = string.Empty;
                    if (video.Message != null)
                    {
                        title = WordHelper.GetSplitByWord(GetTitle(video.Message), 100);
                        description = GetDescription(video.Message);
                    }


                    var result = await _uploader.UploadAsync(title, description, video.LocalPath, stoppingToken);
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

        /// <summary>
        /// Get full description for video on all languages
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string GetDescription(Message message)
        {
            var description = new StringBuilder(512);

            var defaultContent = message.Translates?.FirstOrDefault(x => x.Language == DefaultLanguage);
            if (defaultContent != null)
            {
                description.Append(defaultContent.Message.Content);
            }

            if (description.Length > 0) description.Append("\r\n\r\n");
            description.Append(message.Content);

            if (message.Translates == null)
            {
                return description.ToString();
            }

            foreach (var translate in message.Translates.Where(x => x.Language != DefaultLanguage))
            {
                if (description.Length > 0)
                {
                    description.Append("\r\n\r\n");
                }

                description.Append(translate.Content);
            }


            return description.ToString();
        }

        private string GetTitle(Message message)
        {
            if (message == null)
            {
                return DefaultTitle;
            }

            var translate = message.Translates?.FirstOrDefault(x => x.Language == DefaultLanguage);
            if (translate != null)
            {
                return translate.Content;
            }

            return message.Content;
        }
    }
}
