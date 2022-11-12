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
            await ProcessFileTranslate(file);
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

        Regex regexpoint = new Regex(@"[\-\.]+121[\-\.]+[\d]+(\.)");
        if (regexpoint.IsMatch(content))
        {
            content = regexpoint.Replace(content, ProcessPoint);
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
