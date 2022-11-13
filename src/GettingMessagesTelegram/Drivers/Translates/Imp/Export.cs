using System.Text.RegularExpressions;
using GettingMessagesTelegram.Drivers.Translates.Config;
using GettingMessagesTelegram.Services;
using Google.Apis.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GettingMessagesTelegram.Drivers.Translates.Imp;

public class Export : IExport
{
    private readonly IMessageService _messageService;
    private readonly ILogger<Export> _logger;
    private readonly TranslatesConfig _config;

    private readonly Regex _regexFilename = new Regex(@"(?<messageid>\d)+_(?<commentscount>\d)+_page_(?<page>\d)+\.lng");

    public Export(IOptions<TranslatesConfig> config, IMessageService messageService, ILogger<Export> logger)
    {
        _messageService = messageService;
        _logger = logger;
        _config = config.Value;
    }

    public async Task ExportAsync(CancellationToken cancellation = default)
    {
        foreach (var language in _config.DestLanguages)
        {
            await ProcessLanguage(language);
        }
    }

    private async Task ProcessLanguage(string language)
    {
        var path = GetPath(language);
        if (!Directory.Exists(path))
        {
            _logger.LogInformation($"Export: directory was not find: {path}");
            return;
        }
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            if (_regexFilename.IsMatch(file))
            {
                var match = _regexFilename.Match(file);

                if (!long.TryParse(match.Groups["messageid"].Value, out var messageId))
                {
                    _logger.LogWarning($"Export: invalid message id in filename: {file}, value: {match.Groups["messageid"].Value}");
                    continue;
                }

                if (!long.TryParse(match.Groups["commentscount"].Value, out var commentsCount))
                {
                    _logger.LogWarning($"Export: invalid message comments count in filename: {file}, value: {match.Groups["commentscount"].Value}");
                    continue;
                }

                if (!int.TryParse(match.Groups["page"].Value, out var page))
                {
                    _logger.LogWarning($"Export: invalid message page in filename: {file}, value: {match.Groups["page"].Value}");
                    continue;
                }

                await ProcessFileTranslate(language,file, messageId, commentsCount, page);
            }
        }
    }

    private async Task ProcessFileTranslate(string language, string filename, long messageId, long commentsCount, int page)
    {
        var content = await File.ReadAllTextAsync(filename);

        // cleaning after translate
        content = CleanSeparateString(content);

        var collections = Regex.Split(content, TranslatesConfig.FormatSeparate, RegexOptions.Singleline)
            .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        _logger.LogInformation($"Export: {filename} loaded rows: {collections.Count}");

        // check correct after translate

        var count = page == 0 ? collections.Count - 1 : collections.Count;

        if (count != commentsCount)
        {
            _logger.LogInformation($"Export: {filename} has invalid count of comments need {commentsCount}, but exists: {count}. Check cleaning after translating");
            return;
        }

    }

    /// <summary>
    /// Get path for language where saved translates
    /// </summary>
    private string GetPath(string language)
    {
        return Path.Combine(_config.DestPath, language);
    }

    /// <summary>
    /// Cleaning separating string
    /// </summary>
    public static string CleanSeparateString(string content)
    {
        content = content.Replace("\u200B", "");

        Regex regexComment =
            new Regex(@"[\-]+(?<space>\s)*[\-]*(?<space>\s)*[\-]+(?<space>\s)*[\d]+(?<space>\s)*[\-]+(?<space>\s)*[\-]+(?<space>\s)*[\-]");
        if (regexComment.IsMatch(content))
        {
            content = regexComment.Replace(content, ProcessSpace);
        }

        Regex firstpoint = new Regex(@"[\-][\-\.\]\?\s]*[\d]+[\-\s]*");
        if (firstpoint.IsMatch(content))
        {
            content = firstpoint.Replace(content, ProcessSpace);
        }

        Regex anywords = new Regex(@"[\-][a-zA-Zа-яА-Я\-\.\]\?\s]+[\d]+[\-\s]*");
        if (anywords.IsMatch(content))
        {
            content = anywords.Replace(content, ProcessWords);
        }

        return content;
    }

    /// <summary>
    /// Clear double spaces
    /// </summary>
    private static string ProcessSpace(Match m)
    {
        return m.Value
            .Replace(" ", "")
            .Replace(".", "")
            .Replace("]", "")
            .Replace("?", "");
    }

    /// <summary>
    /// Process replace any word
    /// </summary>
    /// <param name="m">Match</param>
    private static string ProcessWords(Match m)
    {
        var ss = m.Value
            .Replace(" ", "")
            .Replace(".", "")
            .Replace("]", "")
            .Replace("?", "");
        return Regex.Replace(ss, "[a-zA-Zа-яА-Я]*", "");
    }

    /// <summary>
    /// Clear double spaces
    /// </summary>
    private static string ProcessPoint(Match m)
    {
        return m.Value.Replace(".", "-");
    }
}
