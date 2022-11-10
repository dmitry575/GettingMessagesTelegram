using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.DataAccess;
using Microsoft.EntityFrameworkCore;

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
                return (m, true);
            }

            if (message.GroupId.HasValue && message.GroupId > 0)
            {
                m = await GetByGroupId(message.ChannelId, message.GroupId.Value, cancellationToken);
                if (m != null)
                {
                    m.ViewCount = message.ViewCount;
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

    public Task<Message> GetById(long id)
    {
        return _messagesContext
            .Messages
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Message>> GetNotTranslate(string language, int page, int countRows)
    {
        return await _messagesContext
            .Messages
            .AsQueryable()
            .Include(x => x.Comments)
            .Include(x => x.Translates)
            .Where(x => x.Translates == null || x.Translates.All(t => t.Language != language))
            .Skip(page * countRows)
            .Take(countRows)
            .ToListAsync();
    }
}
