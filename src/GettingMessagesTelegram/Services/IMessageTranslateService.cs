using GettingMessagesTelegram.Data;

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

    /// <summary>
    /// Get not sent message
    /// </summary>
    /// <param name="lastId">Id message from get rows</param>
    /// <param name="countRows">Count of row on witch page</param>
    Task<List<MessageTranslate>> GetNotSent(long lastId, int countRows, CancellationToken cancellationToken);

    /// <summary>
    /// Update field DatePublish
    /// </summary>
    /// <param name="id">id message</param>
    Task<int> UpdateDatePublish(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Update field DatePublish and set null
    /// </summary>
    /// <param name="id">id message</param>
    Task<int> UpdateDatePublishToNull(long messageId, string lang, CancellationToken cancellationToken);
}

