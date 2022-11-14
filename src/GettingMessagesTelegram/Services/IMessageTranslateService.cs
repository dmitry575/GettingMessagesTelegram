namespace GettingMessagesTelegram.Services;

/// <summary>
/// Process for translate
/// </summary>
public interface IMessageTranslateService
{
    /// <summary>
    /// Replace translate content
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="content">New content of translate</param>
    /// <param name="language">Language</param>
    /// <param name="cancellationToken"></param>
    Task ReplaceTranslateAsync(long messageId, string content, string language, CancellationToken cancellationToken);
}

