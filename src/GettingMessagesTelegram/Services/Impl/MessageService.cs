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

    public Task<Message> GetByBaseId(long channelId, long baseId)
    {
        return _messagesContext
            .Messages
            .AsQueryable()
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.ChannelId == channelId && x.BaseId == baseId);
    }

    public async Task<int> ReplaceAsync(Message message, CancellationToken cancellationToken)
    {
        if (message.Id <= 0)
        {
            await _messagesContext.Messages.AddAsync(message, cancellationToken);
        }

        return await _messagesContext.SaveChangesAsync(cancellationToken);
    }
}
