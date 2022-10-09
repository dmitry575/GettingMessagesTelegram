using GettingMessagesTelegram.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace GettingMessagesTelegram.Services.Impl;

public class MediaService : IMediaService
{
    private readonly MessagesContext _messagesContext;

    public MediaService(MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }

    public async Task UpdateOrCreate(DataAccess.Media media)
    {
        var m = await _messagesContext
            .Medias
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.BaseId == media.BaseId);
        if (m == null)
        {
            await _messagesContext
                .Medias.AddAsync(media);
            await _messagesContext.SaveChangesAsync();
        }
    }
}
