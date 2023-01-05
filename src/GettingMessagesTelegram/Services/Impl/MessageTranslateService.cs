using GettingMessagesTelegram.DataAccess;
using GettingMessagesTelegram.Data;
using Microsoft.EntityFrameworkCore;

namespace GettingMessagesTelegram.Services.Impl;

public class MessageTranslateService : IMessageTranslateService
{
    private readonly MessagesContext _messagesContext;
    public MessageTranslateService(MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }

    public async Task ReplaceTranslateAsync(long messageId, string content, string language, CancellationToken cancellationToken)
    {
        var message = await _messagesContext.MessagesTranslates
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.MessageId == messageId && x.Language == language, cancellationToken: cancellationToken);

        if (message == null)
        {
            await _messagesContext.MessagesTranslates.AddAsync(new MessageTranslate
            {
                MessageId = messageId,
                Language = language,
                Content = content,
                DateCreated = DateTime.UtcNow
            }, cancellationToken);
        }
        else
        {
            message.Content = content;
        }

        await _messagesContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<MessageTranslate>> GetNotSent(long lastId, int countRows, CancellationToken cancellationToken)
    {
        return await _messagesContext
             .MessagesTranslates
             .AsQueryable()
             .AsNoTracking()
             .Include(x => x.Message)
             .Include(x => x.Message.Medias)
             .Where(x => x.Id > lastId)
             .Where(x => x.PublishData == null)
             .OrderBy(x => x.Id)
             .Take(countRows)
             .ToListAsync(cancellationToken);
    }

    public async Task<int> UpdateDatePublish(long id, CancellationToken cancellationToken)
    {
        var message = await _messagesContext
            .MessagesTranslates
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (message != null)
        {
            message.PublishData = DateTime.UtcNow;
            return await _messagesContext.SaveChangesAsync(cancellationToken);
        }

        return -1;
    }

    public async Task<int> UpdateDatePublishToNull(long messageId, string lang, CancellationToken cancellationToken)
    {
        var message = await _messagesContext
             .MessagesTranslates
             .AsQueryable()
             .FirstOrDefaultAsync(x => x.MessageId == messageId && x.Language == lang);

        if (message != null)
        {
            message.PublishData = null;
            return await _messagesContext.SaveChangesAsync(cancellationToken);
        }

        return -1;
    }
}
