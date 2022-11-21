using System.Text;
using GettingMessagesTelegram.Drivers.Translates.Config;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GettingMessagesTelegram.Drivers.Translates.Imp;

public class Import : IImport
{
    /// <summary>
    /// Count of messages one one page
    /// </summary>
    private const int CountRows = 100;

    /// <summary>
    /// Max file size for translating
    /// </summary>
    private const int MaxFilesize = 3000;

    private readonly IMessageService _messageService;
    private readonly ICommentsService _commentsService;
    private readonly TranslatesConfig _config;
    private readonly ILogger<Import> _logger;
    private readonly IMessageTranslateService _messageTranslateService;

    public Import(IOptions<TranslatesConfig> config, IMessageService messageService, ILogger<Import> logger, ICommentsService commentsService, IMessageTranslateService messageTranslateService)
    {
        _messageService = messageService;
        _logger = logger;
        _commentsService = commentsService;
        _messageTranslateService = messageTranslateService;
        _config = config.Value;
    }

    public async Task ImportAsync(CancellationToken cancellation = default)
    {

        foreach (var language in _config.DestLanguages)
        {

            _logger.LogInformation($"processing messages for languages {language}");

            await UpdateTranslateEmptyContent(language);

            await ImportMessagesNotTranslate(language);

            await ImportCommentsNotTranslate(language);

            _logger.LogInformation($"process messages for languages {language} finished");
        }
    }

    private async Task UpdateTranslateEmptyContent(string language)
    {
        var page = 0;
        var lastId = -1L;
        while (await _messageService.GetNotTranslateEmptyContent(language, lastId, CountRows * 5) is { } messages)
        {
            if (messages.Count <= 0)
            {
                _logger.LogInformation($"not message with empty content for language {language}, page: {page}");
                break;
            }

            foreach (var message in messages)
            {
                await _messageTranslateService.ReplaceTranslateAsync(message.Id, message.Content, language, default);
                _logger.LogInformation($"added empty translate message {message.Id} for language {language}");
                lastId = message.Id;
            }

            page++;
        }
    }

    private async Task ImportCommentsNotTranslate(string language)
    {
        // get comments from translated messages, but have not still translated
        var page = 0;
        var text = new StringBuilder(3048);

        while (await _commentsService.GetNotTranslate(language, page, CountRows * 5) is { } comments)
        {
            if (comments.Count <= 0)
            {
                _logger.LogInformation($"not comments for language {language}, page: {page}");
                break;
            }

            text.Clear();

            var commentsCount = 0;
            var messageId = -1L;
            // must be > 0, because if page == 0 must be content of message

            var commentPage = 1;
            foreach (var comment in comments)
            {
                if (messageId < 0) messageId = comment.MessageId;

                if (messageId != comment.MessageId)
                {
                    SaveToFile(text.ToString(), language, messageId, commentsCount, commentPage); 
                    commentsCount = 0;
                    commentPage = 1;
                    messageId = comment.MessageId;
                }

                text.Append(comment.Content);
                text.Append("\r\n\r\n");
                text.AppendFormat(TranslatesConfig.FormatSeparate, comment.Id);
                text.Append("\r\n\r\n ");

                commentsCount++;

                if (text.Length > MaxFilesize)
                {
                    SaveToFile(text.ToString(), language, messageId, commentsCount, commentPage);
                    text.Clear();
                    commentPage++;
                    commentsCount = 1;
                }
            }
            if (text.Length > 0)
            {
                SaveToFile(text.ToString(), language, messageId, commentsCount, commentPage);
            }

            page++;
        }
    }

    private async Task ImportMessagesNotTranslate(string language)
    {
        var page = 0;
        var text = new StringBuilder(1024);
        while (await _messageService.GetNotTranslate(language, page, CountRows) is { } messages)
        {
            if (messages.Count <= 0)
            {
                _logger.LogInformation($"not messages for language {language}, page: {page}");
                break;
            }
            foreach (var message in messages)
            {

                text.Clear();

                text.Append(message.Content);
                text.Append("\r\n\r\n");
                text.AppendFormat(TranslatesConfig.FormatSeparate, message.Id);
                text.Append("\r\n\r\n ");

                var commentPage = 0;
                var commentsCount = 0;

                var comments = await _commentsService.GetNotTranslate(language, message.Id);

                foreach (var messageComment in comments)
                {
                    text.Append(messageComment.Content);
                    text.Append("\r\n\r\n");
                    text.AppendFormat(TranslatesConfig.FormatSeparate, messageComment.Id);
                    text.Append("\r\n\r\n ");
                    commentsCount++;

                    if (text.Length > MaxFilesize)
                    {
                        SaveToFile(text.ToString(), language, message.Id, commentsCount, commentPage);
                        text.Append("\r\n\r\n ");
                        text.Clear();
                        commentPage++;
                        commentsCount = 0;
                    }
                }

                if (text.Length > 0)
                {
                    SaveToFile(text.ToString(), language, message.Id, commentsCount, commentPage);
                }
            }

            page++;
        }
    }

    private string GetFilename(string language, long messageId, int commentsCount, int page)
    {
        return Path.Combine(_config.SourcePath, language, $"{messageId}_{commentsCount}_page_{page}.lng");
    }

    /// <summary>
    /// Saving content to file
    /// </summary>
    private void SaveToFile(string text, string language, long messageId, int commentsCount, int page)
    {
        var fileName = GetFilename(language, messageId, commentsCount, page);

        if (File.Exists(fileName))
        {
            _logger.LogInformation($"file exists {fileName}");
            return;
        }

        if (string.IsNullOrEmpty(text))
        {
            _logger.LogInformation($"content is empty for file {fileName}");
            return;
        }
        var path = Path.Combine(_config.SourcePath, language);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            _logger.LogInformation($"directory created {path}");
        }


        if (fileName != null)
        {
            File.WriteAllText(fileName, text);
            _logger.LogInformation($"file saved: {fileName} {text.Length} bytes");
        }
    }
}
