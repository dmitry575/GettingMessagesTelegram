using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace GettingMessagesTelegram.Services.Impl;

public class MessageService : IMessageService
{
    private readonly MessagesContext _messagesContext;

    public MessageService(MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }

    public Task<Message> GetByBaseId(long channelId, long baseId, CancellationToken cancellationToken)
    {
        return _messagesContext
            .Messages
            .AsQueryable()
            .Include(x => x.Comments)
            .Include(x => x.Medias)
            .FirstOrDefaultAsync(x => x.ChannelId == channelId && x.BaseId == baseId, cancellationToken: cancellationToken);
    }

    public async Task<int> ReplaceAsync(Message message, CancellationToken cancellationToken)
    {
        if (message.Id <= 0)
        {
            await _messagesContext.Messages.AddAsync(message, cancellationToken);
        }

        return await _messagesContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Message> GetByGroupId(long channelId, long groupId, CancellationToken cancellationToken)
    {
        return _messagesContext
            .Messages
            .AsQueryable()
            .Include(x => x.Comments)
            .Include(x => x.Medias)
            .FirstOrDefaultAsync(x => x.ChannelId == channelId && x.GroupId == groupId, cancellationToken: cancellationToken);
    }

    public async Task<(Message, bool)> GetCreateByMessage(Message message, CancellationToken cancellationToken)
    {
        await using var transaction = await _messagesContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var m = await GetByBaseId(message.ChannelId, message.BaseId, cancellationToken);
            if (m is not null)
            {
                m.ViewCount = message.ViewCount;
                if (string.IsNullOrEmpty(m.Content) && !string.IsNullOrEmpty(message.Content) && message.Content.Length > 2)
                {
                    m.Content = message.Content;
                    await transaction.CommitAsync(cancellationToken);
                }
                return (m, true);
            }

            if (message.GroupId is > 0)
            {
                m = await GetByGroupId(message.ChannelId, message.GroupId.Value, cancellationToken);
                if (m != null)
                {
                    m.ViewCount = message.ViewCount;
                    if (string.IsNullOrEmpty(m.Content) && !string.IsNullOrEmpty(message.Content) && message.Content.Length > 2)
                    {
                        m.Content = message.Content;
                    }
                    return (m, false);
                }
            }

            await ReplaceAsync(message, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return (message, false);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task<Message> GetById(long id, bool withTranslates = false)
    {
        var query = _messagesContext
            .Messages
            .AsNoTracking()
            .AsQueryable();
        if (withTranslates)
        {
            query = query.Include(x => x.Translates);
        }

        return query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Message>> GetNotTranslate(string language, int page, int countRows)
    {
        return await _messagesContext
            .Messages
            .AsQueryable()
            .AsNoTracking()
            .Include(x => x.Translates)
            .Where(x => !x.Translates.Any() || x.Translates.All(t => t.Language != language))
            .OrderBy(x => x.Id)
            .Skip(page * countRows)
            .Take(countRows)
            .ToListAsync();
    }

    public async Task<List<Message>> GetNotTranslateEmptyContent(string language, long lastId, int countRows)
    {
        return await _messagesContext
            .Messages
            .AsQueryable()
            .AsNoTracking()
            .Include(x => x.Translates)
            .Where(x => x.Id > lastId)
            .Where(x => x.Content == "" || x.Content == null)
            .Where(x => !x.Translates.Any() || x.Translates.All(t => t.Language != language))
            .OrderBy(x => x.Id)
            .Take(countRows)
            .ToListAsync();
    }

    public async Task<List<Message>> GetNotSent(long lastId, int countRows, CancellationToken cancellationToken)
    {
        return await _messagesContext
            .Messages
            .AsQueryable()
            .Include(x => x.Medias)
            .Where(x => x.Id > lastId)
            .Where(x => x.PublishData == null)
            .OrderBy(x => x.Id)
            .Take(countRows)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> UpdateDatePublish(long id, CancellationToken cancellationToken)
    {
        var message = await _messagesContext
             .Messages
             .AsQueryable()
             .FirstOrDefaultAsync(x => x.Id == id);

        if (message != null)
        {
            message.PublishData = DateTime.UtcNow;
            return await _messagesContext.SaveChangesAsync(cancellationToken);
        }

        return -1;
    }

    public async Task<int> UpdateDatePublishToNull(long id, CancellationToken cancellationToken)
    {
        var message = await _messagesContext
             .Messages
             .AsQueryable()
             .FirstOrDefaultAsync(x => x.Id == id);

        if (message != null)
        {
            message.PublishData = null;
            return await _messagesContext.SaveChangesAsync(cancellationToken);
        }

        return -1;
    }
}
