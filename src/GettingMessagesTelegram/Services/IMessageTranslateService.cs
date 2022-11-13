namespace GettingMessagesTelegram.Services;

/// <summary>
/// Process for translate
/// </summary>
public interface IMessageTranslateService
{
    Task ReplaceTranslateAsync(long messageId, string content, string language, CancellationToken cancellationToken);
}

