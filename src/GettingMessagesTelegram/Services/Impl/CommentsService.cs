using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GettingMessagesTelegram.Services.Impl;

public class CommentsService : ICommentsService
{
    private readonly MessagesContext _messagesContext;

    public CommentsService(MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }



    public async Task<List<Comment>> GetNotTranslate(string language, int page, int countRows)
    {
        return await _messagesContext
            .Comments
            .AsQueryable()
            .AsNoTracking()
            .Include(x => x.Translates)
            .Include(x => x.Message)
            .Include(x => x.Message.Translates)
            .Where(x => x.Message.Translates.Any() && x.Message.Translates.Any(t => t.Language == language))
            .Where(x => !x.Translates.Any() || x.Translates.All(t => t.Language != language))
            .OrderBy(x => x.MessageId)
            .Skip(page * countRows)
            .Take(countRows)
            .ToListAsync();
    }

    public async Task<List<Comment>> GetNotTranslate(string language, long messageId)
    {
        return await _messagesContext
            .Comments
            .AsNoTracking()
            .Include(x => x.Translates)
            .Where(x => x.MessageId == messageId)
            .ToListAsync();
    }

    public async Task<List<Comment>> GetNotSent(long lastId, int countRows, CancellationToken cancellationToken)
    {
        return await _messagesContext
            .Comments
            .AsQueryable()
            .Include(x => x.Message)
            .Where(x => x.Message.PublishData != null)
            .Where(x => x.Id > lastId)
            .Where(x => x.PublishData == null)
            .OrderBy(x => x.Id)
            .Take(countRows)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> UpdateDatePublish(long id, CancellationToken cancellationToken)
    {
        var comment = await _messagesContext
            .Comments
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (comment != null)
        {
            comment.PublishData = DateTime.UtcNow;
            return await _messagesContext.SaveChangesAsync(cancellationToken);
        }

        return -1;
    }

    public async Task<int> UpdateDatePublish(long[] id, CancellationToken cancellationToken)
    {
        var comments = await _messagesContext
            .Comments
            .AsQueryable()
            .Where(x => id.Contains(x.Id))
            .ToListAsync();

        if (comments != null)
        {
            comments.ForEach(a => a.PublishData = DateTime.UtcNow);
            return await _messagesContext.SaveChangesAsync(cancellationToken);
        }

        return -1;
    }
}

