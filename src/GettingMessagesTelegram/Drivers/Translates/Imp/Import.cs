using System.IO;
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
    private readonly TranslatesConfig _config;
    private readonly ILogger<Import> _logger;

    public Import(IOptions<TranslatesConfig> config, IMessageService messageService, ILogger<Import> logger)
    {
        _messageService = messageService;
        _logger = logger;
        _config = config.Value;
    }

    public async Task ImportAsync(CancellationToken cancellation = default)
    {
        var messageIds = new HashSet<long>();

        foreach (var language in _config.DestLanguages)
        {
            var page = 0;
            _logger.LogInformation($"processing messages for languages {language}");

            while (await _messageService.GetNotTranslate(language, page, CountRows) is { } messages)
            {
                if (messages.Count <= 0)
                {
                    _logger.LogInformation($"not messages for language {language}, page: {page}");
                    break;
                }
                foreach (var message in messages)
                {
                    messageIds.Add(message.Id);

                    var text = new StringBuilder(1024);

                    text.Append(message.Content);
                    text.Append("\r\n\r\n");
                    text.AppendFormat(TranslatesConfig.FormatSeparate, message.Id);
                    text.Append("\r\n\r\n ");

                    var commentPage = 0;
                    var commentsCount = 0;

                    

                    foreach (var messageComment in message.Comments)
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

            _logger.LogInformation($"process messages for languages {language} finished");
        }
    }

    private string GetFilename(string language, long messageId, int commentsCount, int page = 0)
    {
        return Path.Combine(_config.SourcePath, language, $"{messageId}_{commentsCount}_page_{page}.lng");
    }

    /// <summary>
    /// Saving content to file
    /// </summary>
    private void SaveToFile(string text, string language, long messageId, int commentsCount, int page)
    {
        var fileName = GetFilename(language, messageId, commentsCount, page);

        //if (File.Exists(fileName))
        //{
        //    _logger.LogInformation($"file exists {fileName}");
        //    return;
        //}
        //if (!Directory.Exists(_config.SourcePath))
        //{
        //    Directory.CreateDirectory(_config.SourceLanguage);
        //    _logger.LogInformation($"directory created {_config.SourceLanguage}");
        //}

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
