using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Services;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using TL;
using TranslateService.Config;
using TranslateService.Requests;
using TranslateService.Responses;

namespace TranslateService.Services.Impl
{
    public class TranslateMessages : ITranslateMessages
    {
        /// <summary>
        /// Count of messages one one page
        /// </summary>
        private const int CountRows = 100;

        /// <summary>
        /// minimal leghth of translated message
        /// </summary>
        private const int MinLengthMessage = 10;

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
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            _httpClient.Timeout = TimeSpan.FromSeconds(300);

        }

        public async Task Translate(CancellationToken stoppingToken)
        {
            foreach (var language in _translateConfig.DestLanguages)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                await TranslateLanguage(language, stoppingToken);
            }

        }

        private async Task TranslateLanguage(string lang, CancellationToken token)
        {
            var page = 0;

            try
            {
                while (await _messageService.GetNotTranslate(lang, page, CountRows) is { } messages)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    if (messages.Count <= 0)
                    {
                        _logger.LogInformation($"not messages for language {lang}, page: {page}");
                        break;
                    }
                    foreach (var message in messages)
                    {
                        if (!await TranslateMessage(message, lang, token))
                        {
                            continue;
                        }

                        _logger.LogInformation($"get comments of messages for language {lang}, message id: {message.Id}");
                        var comments = await _commentsService.GetNotTranslate(lang, message.Id);

                        foreach (var messageComment in comments)
                        {
                            if (token.IsCancellationRequested)
                            {
                                return;
                            }

                            await TranslateComment(messageComment, lang, token);
                        }

                    }

                    page++;
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($"translate messages failed: language: {lang}, page: {page}, {e}");

            }
        }

        private async Task TranslateComment(GettingMessagesTelegram.Data.Comment comment, string lang, CancellationToken token)
        {
            try
            {
                var request = new TranslateRequest
                {
                    Text = comment.Content,
                    FromLang = _translateConfig.SourceLanguage,
                    ToLang = lang,
                    IsHtml = false,
                    Convert = false
                };

                _logger.LogInformation($"sending translate comment {comment.Id} of message: {comment.MessageId}");

                var content = new StringContent(JsonConvert.SerializeObject(request));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using var postMessage = await _httpClient.PostAsync(_translateConfig.Url, content, token);

                if (postMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"translate message failed: status code: {postMessage.StatusCode}");
                    return;
                }

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

        private async Task<bool> TranslateMessage(GettingMessagesTelegram.Data.Message message, string lang, CancellationToken token)
        {
            try
            {
                var request = new TranslateRequest
                {
                    Text = message.Content,
                    FromLang = _translateConfig.SourceLanguage,
                    ToLang = lang,
                    IsHtml = false,
                    Convert = true
                };
                _logger.LogInformation($"sending translate message {message.Id}");

                var content = new StringContent(JsonConvert.SerializeObject(request));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using var postMessage = await _httpClient.PostAsync(_translateConfig.Url, content, token);

                if (postMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"translate message failed: status code: {postMessage.StatusCode}");
                    return false;
                }
                _logger.LogInformation($"response send translate to {_translateConfig.Url}, message: {message.Id}");

                var json = await postMessage.Content.ReadAsStringAsync(token);
                _logger.LogInformation($"response send translate: {json}");

                var response = JsonConvert.DeserializeObject<TranslateResponse>(json);

                if (response?.TextTranslated != null && !string.IsNullOrEmpty(response.TextTranslated) && response.TextTranslated.Length > MinLengthMessage)
                {
                    await _messageTranslateService.ReplaceTranslateAsync(message.Id, response.TextTranslated, lang, token);
                    _logger.LogInformation($"message tanslated: {message.Id}");
                    return true;
                }
                else
                {
                    _logger.LogInformation($"message tanslated not saved: {message.Id}, is empty: {string.IsNullOrEmpty(response?.TextTranslated)}, lenght less: {response.TextTranslated.Length <= MinLengthMessage}");
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"translate message failed: {e}");

            }
            return false;
        }
    }
}
