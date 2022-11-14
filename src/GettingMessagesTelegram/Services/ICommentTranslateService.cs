namespace GettingMessagesTelegram.Services;

/// <summary>
/// Comments translate 
/// </summary>
public interface ICommentTranslateService
{
    Task ReplaceTranslateAsync(long commentId, string content, string language, CancellationToken cancellationToken);
}
