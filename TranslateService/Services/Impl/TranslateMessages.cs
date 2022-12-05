using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Drivers.PostImage.Models;
using GettingMessagesTelegram.Drivers.Translates.Config;
using GettingMessagesTelegram.Services;
using Google.Apis.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using TL;
using TranslateService.Config;
using TranslateService.Responses;
using static System.Net.Mime.MediaTypeNames;

namespace TranslateService.Services.Impl
{
    public class TranslateMessages : ITranslateMessages
    {
        /// <summary>
        /// Count of messages one one page
        /// </summary>
        private const int CountRows = 100;

        private readonly HttpClient _httpClient;
        private readonly TranslateConfig _translateConfig;
        private readonly IMessageService _messageService;
        private readonly ICommentsService _commentsService;
        private readonly ILogger<TranslateMessages> _logger;

        public TranslateMessages(HttpClient httpClient, IOptions<TranslateConfig> translateConfig, IMessageService messageService, ICommentsService commentsService,
            ILogger<TranslateMessages> logger)
        {
            _httpClient = httpClient;
            _translateConfig = translateConfig.Value;
            _messageService = messageService;
            _commentsService = commentsService;
            _logger = logger;
        }

        public async Task Translate(CancellationToken stoppingToken)
        {
            foreach (var language in _translateConfig.DestLanguages)
            {
                await TranslateLanguage(language, stoppingToken);
            }

        }

        private async Task TranslateLanguage(string lang, CancellationToken token)
        {
            var page = 0;
            while (await _messageService.GetNotTranslate(lang, page, CountRows) is { } messages)
            {
                if (messages.Count <= 0)
                {
                    _logger.LogInformation($"not messages for language {lang}, page: {page}");
                    break;
                }
                foreach (var message in messages)
                {
                    var content = new FormUrlEncodedContent(new[]
                        {
                             new KeyValuePair<string, string>("text", message.Content),
                             new KeyValuePair<string, string>("fromLang", _translateConfig.SourceLanguage),
                             new KeyValuePair<string, string>("toLang", lang),
                             new KeyValuePair<string, string>("isHtml", "false"),
                             new KeyValuePair<string, string>("convert", "false")
                        });

                    using var postMessage = await _httpClient.PostAsync(_translateConfig.Url, content);
                    _logger.LogInformation($"response send translate to _translateConfig.Url, message: {message.Id}");

                    var json = await postMessage.Content.ReadAsStringAsync();
                    _logger.LogInformation($"response send translate: {json}");

                    var response = JsonConvert.DeserializeObject<TranslateResponse>(json);


                    var comments = await _commentsService.GetNotTranslate(language, message.Id);

                    foreach (var messageComment in comments)
                    {

                    }


                }

                page++;
            }
        }
    }
}
