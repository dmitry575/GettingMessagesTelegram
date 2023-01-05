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

    public async Task<List<CommentTranslate>> GetNotSent(long lastId, int countRows, CancellationToken cancellationToken)
    {
        return await _messagesContext
             .CommentsTranslates
             .AsQueryable()
             .Include(x => x.Comment)
             .Include(x => x.Comment.Message)
             .Include(x => x.Comment.Message.Translates)
             .Where(x => x.Comment.Message.Translates.Any(t => t.Language == x.Language && t.PublishData != null))
             .Where(x => x.Id > lastId)
             .Where(x => x.PublishData == null)
             .OrderBy(x => x.Id)
             .Take(countRows)
             .ToListAsync(cancellationToken);
    }

    public async Task<int> UpdateDatePublish(long id, CancellationToken cancellationToken)
    {
        var message = await _messagesContext
            .CommentsTranslates
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (message != null)
        {
            message.PublishData = DateTime.UtcNow;
            return await _messagesContext.SaveChangesAsync(cancellationToken);
        }

        return -1;
    }
}
