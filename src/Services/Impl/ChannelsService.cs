using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace GettingMessagesTelegram.Services.Impl;

public class ChannelsService : IChannelsService
{
    private readonly MessagesContext _messagesContext;

    public ChannelsService(MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }

    public async Task<Channel> CheckAdd(long baseId, long hashAccess, string author)
    {
        var channel = await _messagesContext.Channels
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BaseId == baseId);
        if (channel is null)
        {
            channel = new Channel
            {
                BaseId = baseId,
                Author = author,
                HashAccess = hashAccess
            };
            await _messagesContext.Channels
                .AddAsync(channel);
            await _messagesContext.SaveChangesAsync();
        }

        return channel;
    }
}
