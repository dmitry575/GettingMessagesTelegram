namespace GettingMessagesTelegram.Services;
public interface ICommentTranslateService
{
    Task ReplaceTranslateAsync(long commentId, string content, string language, CancellationToken cancellationToken);
}
