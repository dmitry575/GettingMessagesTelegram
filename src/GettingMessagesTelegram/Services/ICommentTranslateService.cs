using GettingMessagesTelegram.Data;

namespace GettingMessagesTelegram.Services;

/// <summary>
/// Comments translate 
/// </summary>
public interface ICommentTranslateService
{
    /// <summary>
    /// Replace: update or add translate
    /// </summary>
    Task ReplaceTranslateAsync(long commentId, string content, string language, CancellationToken cancellationToken);

    /// <summary>
    /// Get not sent comments
    /// </summary>
    /// <param name="lastId">Id message from get rows</param>
    /// <param name="countRows">Count of row on witch page</param>
    Task<List<CommentTranslate>> GetNotSent(long lastId, int countRows, CancellationToken cancellationToken);

    /// <summary>
    /// Update field DatePublish
    /// </summary>
    /// <param name="id">id comment</param>
    Task<int> UpdateDatePublish(long id, CancellationToken cancellationToken);
}
