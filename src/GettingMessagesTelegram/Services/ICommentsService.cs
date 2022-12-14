using GettingMessagesTelegram.Data;

namespace GettingMessagesTelegram.Services;

/// <summary>
/// Work for comments
/// </summary>
public interface ICommentsService
{
    /// <summary>
    /// Get not translate messages
    /// </summary>
    /// <param name="language">Language for searching not translating messages</param>
    /// <param name="page">Current page</param>
    /// <param name="countRows">Count of row on witch page</param>
    Task<List<Comment>> GetNotTranslate(string language, int page, int countRows);

    /// <summary>
    /// Get not translate messages for one message
    /// </summary>
    /// <param name="language">Language for searching not translating messages</param>
    /// <param name="messageId">Current id message</param>
    Task<List<Comment>> GetNotTranslate(string language, long messageId);

    /// <summary>
    /// Get not sent comments
    /// </summary>
    /// <param name="lastId">Id message from get rows</param>
    /// <param name="countRows">Count of row on witch page</param>
    Task<List<Comment>> GetNotSent(long lastId, int countRows, CancellationToken cancellationToken);

    /// <summary>
    /// Update field DatePublish
    /// </summary>
    /// <param name="id">id comment</param>
    Task<int> UpdateDatePublish(long id, CancellationToken cancellationToken);
}
