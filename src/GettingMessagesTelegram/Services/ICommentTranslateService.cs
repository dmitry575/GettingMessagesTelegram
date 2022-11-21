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
}
