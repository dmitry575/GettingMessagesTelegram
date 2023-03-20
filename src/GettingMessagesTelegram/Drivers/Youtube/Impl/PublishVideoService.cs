using System.Text;
using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Drivers.PostImage.Impl;
using GettingMessagesTelegram.Drivers.Youtube.Config;
using GettingMessagesTelegram.Helpers;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GettingMessagesTelegram.Drivers.Youtube.Impl
{
    public class PublishVideoService : IPublishVideoService
    {

        /// <summary>
        /// Default language fro title of video
        /// </summary>
        private const string DefaultLanguage = "en";

        public int MaxLengthDescription = 4500;

        public int MaxLenghtTitle = 100;

        private const int Rows = 20;
        private readonly IMediaService _mediaService;
        private readonly IYouTubeUploader _uploader;
        private readonly ILogger<PublishMediaService> _logger;
        private readonly YoutubeConfig _config;
        public PublishVideoService(IMediaService mediaService, 
            IYouTubeUploader uploader,
            IOptions<YoutubeConfig> config,
            ILogger<PublishMediaService> logger)
        {
            _mediaService = mediaService;
            _uploader = uploader;
            _logger = logger;
            _config = config.Value;
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

                    var title = _config.DefaultTitle;
                    var description = string.Empty;
                    if (video.Message != null)
                    {
                        title = WordHelper.GetSplitByWord(GetTitle(video.Message), MaxLenghtTitle, WordHelper.DelimitaryTitle);
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
                description.Append(defaultContent.Content);
            }

            if (description.Length > 0)
            {
                description.Append("\r\n\r\n");
            }
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

            if (description.Length > MaxLengthDescription)
            {
                return WordHelper.GetSplitByWord(description.ToString(), MaxLengthDescription, WordHelper.DelimitaryTitle);
            }

            return description.ToString();
        }

        private string GetTitle(Message message)
        {
            if (message == null)
            {
                return _config.DefaultTitle;
            }

            var translate = message.Translates?.FirstOrDefault(x => x.Language == DefaultLanguage);
            if (translate != null)
            {
                return string.IsNullOrEmpty(translate.Content) ? _config.DefaultTitle : translate.Content;
            }

            return string.IsNullOrEmpty(message.Content) ? _config.DefaultTitle : message.Content;
        }
    }
}
