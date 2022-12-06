using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using TL;
using TranslateService.Config;
using TranslateService.Responses;

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
        private readonly IMessageTranslateService _messageTranslateService;
        private readonly ICommentTranslateService _commentTranslateService;
        private readonly ILogger<TranslateMessages> _logger;

        public TranslateMessages(HttpClient httpClient,
            IOptions<TranslateConfig> translateConfig,
            IMessageService messageService,
            ICommentsService commentsService,
            IMessageTranslateService messageTranslateService,
            ICommentTranslateService commentTranslateService,
            ILogger<TranslateMessages> logger)
        {
            _httpClient = httpClient;
            _translateConfig = translateConfig.Value;
            _messageService = messageService;
            _commentsService = commentsService;
            _messageTranslateService = messageTranslateService;
            _commentTranslateService = commentTranslateService;
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
                    await TranslateMessage(message, lang, token);


                    var comments = await _commentsService.GetNotTranslate(lang, message.Id);

                    foreach (var messageComment in comments)
                    {
                        await TranslateComment(messageComment, lang, token);
                    }

                }

                page++;
            }
        }

        private async Task TranslateComment(Comment comment, string lang, CancellationToken token)
        {
            try
            {
                //var content = new FormUrlEncodedContent(new[]
                //{
                //             new KeyValuePair<string, string>("text", comment.Content),
                //             new KeyValuePair<string, string>("fromLang", _translateConfig.SourceLanguage),
                //             new KeyValuePair<string, string>("toLang", lang),
                //             new KeyValuePair<string, string>("isHtml", "false"),
                //             new KeyValuePair<string, string>("convert", "false")
                //        });

                var variables = new Dictionary<string, string>() {
    { "text", comment.Content },
    { "fromLang", _translateConfig.SourceLanguage }
};
                var content = new StringContent("text=test&fromLang=en&toLang=ru&isHtml=false&convert=false", Encoding.UTF8);
                
                using var postMessage = await _httpClient.PostAsync(_translateConfig.Url, content, token);
                _logger.LogInformation($"response send translate to {_translateConfig.Url}, comment: {comment.Id}");

                var json = await postMessage.Content.ReadAsStringAsync(token);
                _logger.LogInformation($"response send translate commnet: {json}");

                var response = JsonConvert.DeserializeObject<TranslateResponse>(json);

                if (response?.TextTranslated != null && !string.IsNullOrEmpty(response.TextTranslated))
                {
                    await _commentTranslateService.ReplaceTranslateAsync(comment.Id, response.TextTranslated, lang, token);
                    _logger.LogInformation($"comment tanslated: {comment.Id}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"translate message failed: {e}");
            }
        }

        private async Task TranslateMessage(GettingMessagesTelegram.Data.Message message, string lang, CancellationToken token)
        {
            try
            {
                //var content = new FormUrlEncodedContent(new[]
                //            {
                //             new KeyValuePair<string, string>("text", message.Content),
                //             new KeyValuePair<string, string>("fromLang", _translateConfig.SourceLanguage),
                //             new KeyValuePair<string, string>("toLang", lang),
                //             new KeyValuePair<string, string>("isHtml", "false"),
                //             new KeyValuePair<string, string>("convert", "false")
                //        });
                var content = new StringContent("text=test&fromLang=en&toLang=ru&isHtml=false&convert=false", Encoding.UTF8);

                using var postMessage = await _httpClient.PostAsync(_translateConfig.Url, content, token);
                _logger.LogInformation($"response send translate to _translateConfig.Url, message: {message.Id}");

                var json = await postMessage.Content.ReadAsStringAsync(token);
                _logger.LogInformation($"response send translate: {json}");

                var response = JsonConvert.DeserializeObject<TranslateResponse>(json);

                if (response?.TextTranslated != null && !string.IsNullOrEmpty(response.TextTranslated))
                {
                    await _messageTranslateService.ReplaceTranslateAsync(message.Id, response.TextTranslated, lang, token);
                    _logger.LogInformation($"message tanslated: {message.Id}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"translate message failed: {e}");
            }
        }
    }
}
