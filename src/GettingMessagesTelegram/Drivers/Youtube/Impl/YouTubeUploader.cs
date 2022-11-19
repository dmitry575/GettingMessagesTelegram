using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using System.Reflection;
using Google.Apis.Auth.OAuth2;
using GettingMessagesTelegram.Drivers.Youtube.Config;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using GettingMessagesTelegram.Drivers.Youtube.Models;
using Google.Apis.Upload;

namespace GettingMessagesTelegram.Drivers.Youtube.Impl
{
    /// <summary>
    /// Upload video to youtube channel
    /// Not thread safe
    /// </summary>
    public class YouTubeUploader : IYouTubeUploader
    {
        /// <summary>
        /// Url to youtube
        /// </summary>
        private const string YoutubeUrl = "https://youtu.be/";

        private readonly ILogger<YouTubeUploader> _logger;
        private readonly YoutubeConfig _config;
        private string _videoId = string.Empty;
        public YouTubeUploader(IOptions<YoutubeConfig> config, ILogger<YouTubeUploader> logger)
        {
            _logger = logger;
            _config = config.Value;
        }
        public async Task<UploadResult> UploadAsync(string title, string description, string fileName, CancellationToken cancellation = default)
        {
            _videoId = string.Empty;

            try
            {
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = _config.ClientId,
                        ClientSecret = _config.ClientSecret
                    },
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access.
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    cancellation
                );

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
                });

                var video = new Video
                {
                    Snippet = new VideoSnippet
                    {
                        Title = title,
                        Description = description,
                        Tags = new string[] { "war", "warinukrain", "war2022" },
                        CategoryId = "22" // See https://developers.google.com/youtube/v3/docs/videoCategories/list
                    },
                    Status = new VideoStatus
                    {
                        PrivacyStatus = "public" // or "private" or "public"
                    }
                };


                await using var fileStream = new FileStream(fileName, FileMode.Open);

                var videosInsertRequest =
                    youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                videosInsertRequest.ResponseReceived += VideosInsertRequestResponseReceived;
                videosInsertRequest.ProgressChanged += VideosInsertRequestProgressChanged;

                var result = await videosInsertRequest.UploadAsync(cancellation);

                return new UploadResult
                {
                    Success = result.Status == UploadStatus.Completed,
                    Url = YoutubeUrl + _videoId
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"upload video failed, {fileName}, {title}, {e}");
                return new UploadResult { Success = false };
            }

            void VideosInsertRequestResponseReceived(Video video)
            {
                _logger.LogInformation($"Video id '{video.Id}' was successfully uploaded");
                _videoId = video.Id;
            }

            void VideosInsertRequestProgressChanged(IUploadProgress progress)
            {
                switch (progress.Status)
                {
                    case UploadStatus.Uploading:
                        _logger.LogInformation($"{progress.BytesSent} bytes sent.");
                        break;

                    case UploadStatus.Failed:
                        _logger.LogInformation($"An error prevented the upload from completing, {progress.Exception}");
                        break;
                }
            }

        }
    }
}
