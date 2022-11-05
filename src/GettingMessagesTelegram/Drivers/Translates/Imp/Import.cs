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
        var page = 0;

        foreach (var language in _config.DestLanguages)
        {
            while (await _messageService.GetNotTranslate(language, page, CountRows) is { } messages)
            {
                foreach (var message in messages)
                {
                    var text = new StringBuilder(1024);

                    text.Append(message.Content);
                    text.Append("\r\n\r\n");
                    text.AppendFormat(TranslatesConfig.FormatSeparate, message.Id);
                    text.Append("\r\n\r\n ");

                    foreach (var messageComment in message.Comments)
                    {
                        text.Append(messageComment.Content);
                        text.Append("\r\n\r\n");
                        text.AppendFormat(TranslatesConfig.FormatSeparate, messageComment.Id);
                        text.Append("\r\n\r\n ");
                    }

                    SaveToFile(text.ToString(), language, message.Id, message.Comments.Count);
                }

                page++;
            }
        }
    }

    private string GetFilename(string language, long messageId, int commentsCount, int page = 0)
    {
        return Path.Combine(_config.SourcePath, language, $"{messageId}_{commentsCount}_page_{page}.lng");
    }


    private void SaveToFile(string text, string language, long messageId, int commentsCount)
    {
        var fileName = GetFilename(language, messageId, commentsCount);

        if (File.Exists(fileName))
        {
            _logger.LogInformation($"file exists {fileName}");
            return;
        }
        if (!Directory.Exists(_config.SourcePath))
        {
            Directory.CreateDirectory(_config.SourceLanguage);
            _logger.LogInformation($"directory created {_config.SourceLanguage}");
        }

        var path = Path.Combine(_config.SourcePath, language);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            _logger.LogInformation($"directory created {path}");
        }


        File.WriteAllText(fileName, text);
        _logger.LogInformation($"file saved: {fileName} {text.Length} bytes");
    }
}
