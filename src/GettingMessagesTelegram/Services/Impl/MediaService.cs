﻿using GettingMessagesTelegram.DataAccess;
using GettingMessagesTelegram.Enums;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<Data.Media>> GetPhotosNotSent(long id, int rows, CancellationToken token = default)
    {
        return await _messagesContext
            .Medias
            .AsQueryable()
            .Where(x => x.Id > id)
            .Where(x => x.Type == MediaType.Photo)
            .Where(x => x.UrlExternal == null || x.UrlExternal == "")
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
}
