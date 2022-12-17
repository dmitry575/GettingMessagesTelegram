using GettingMessagesTelegram.DataAccess;
using GettingMessagesTelegram.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GettingMessagesTelegram.Services.Impl;

public class MediaService : IMediaService
{
    private readonly MessagesContext _messagesContext;

    public MediaService(MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }

    public async Task UpdateOrCreate(Data.Media media)
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

    /// <summary>
    /// Get photos not send to website
    /// </summary>
    /// <param name="id"></param>
    /// <param name="rows"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<List<Data.Media>> GetPhotosNotSent(long id, int rows, CancellationToken token = default)
    {
        return await _messagesContext
            .Medias
            .AsNoTracking()
            .Include(x => x.Message)
            .Where(x => x.Id > id)
            .Where(x => x.Type == MediaType.Photo)
            .Where(x => x.UrlExternal == null || x.UrlExternal == "")
            .OrderBy(x => x.Id)
            .Take(rows)
            .ToListAsync(token);

    }

    /// <summary>
    /// Get videos not send to website
    /// </summary>
    public async Task<List<Data.Media>> GetVideosNotSent(long id, int rows, CancellationToken token = default)
    {
        return await _messagesContext
            .Medias
            .AsNoTracking()
            .Include(x => x.Message)
            .Include(x => x.Message.Translates)
            .Where(x => x.Id > id)
            .Where(x => x.Type == MediaType.Video)
            .Where(x => x.UrlExternal == null || x.UrlExternal == "")
            .Where(x => x.Message.Translates.Any(t => t.Language == "en"))
            .OrderBy(x => x.Id)
            .Take(rows)
            .ToListAsync(token);
    }

    public async Task UpdateSend(long mediaId, string urlExternal)
    {
        var media = await _messagesContext
            .Medias
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == mediaId);
        if (media != null)
        {
            media.UrlExternal = urlExternal;
            await _messagesContext.SaveChangesAsync();
        }
    }

    public async Task Delete(long id)
    {
        var media = await _messagesContext
            .Medias
             .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (media != null)
        {
            _messagesContext.Remove(media);
            await _messagesContext.SaveChangesAsync();
        }
    }

    public async Task UpdateDatePublish(long[] mediaIds, CancellationToken token = default)
    {
        var medias = await _messagesContext
            .Medias
            .Where(x => mediaIds.Contains(x.Id))
            .ToListAsync(token);
        
        if (medias != null && medias.Count > 0)
        {
            medias.ForEach(x => x.PublishData = DateTime.UtcNow);
            await _messagesContext.SaveChangesAsync();
        }
    }
}
