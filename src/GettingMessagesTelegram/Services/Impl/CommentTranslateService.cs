using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace GettingMessagesTelegram.Services.Impl;
public class CommentTranslateService : ICommentTranslateService
{
    private readonly MessagesContext _messagesContext;
    public CommentTranslateService(MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }

    public async Task AddTranslateAsync(long commentId, string content, string language)
    {
        await _messagesContext.CommentsTranslates.AddAsync(new CommentTranslate()
        {
            CommentId = commentId,
            Language = language,
            Content = content,
            DateCreated = DateTime.UtcNow
        });
    }

    public async Task ReplaceTranslateAsync(long commentId, string content, string language, CancellationToken cancellationToken)
    {
        var commentTranslate = await _messagesContext.CommentsTranslates
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.CommentId == commentId && x.Language == language, cancellationToken: cancellationToken);

        if (commentTranslate == null)
        {
            await _messagesContext.CommentsTranslates.AddAsync(new CommentTranslate
            {
                CommentId = commentId,
                Language = language,
                Content = content,
                DateCreated = DateTime.UtcNow
            }, cancellationToken);
        }
        else
        {
            commentTranslate.Content = content;
        }

        await _messagesContext.SaveChangesAsync(cancellationToken);
    }
}
