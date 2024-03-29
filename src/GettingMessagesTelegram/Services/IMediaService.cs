﻿namespace GettingMessagesTelegram.Services;

public interface IMediaService
{
    /// <summary>
    /// Create or update media data of message
    /// </summary>
    /// <param name="media">Information of photo or video</param>
    Task UpdateOrCreate(Data.Media media);

    /// <summary>
    /// Get not send images
    /// </summary>
    Task<List<Data.Media>> GetPhotosNotSent(long id, int rows, CancellationToken token = default);

    /// <summary>
    /// Get not send videos
    /// </summary>
    Task<List<Data.Media>> GetVideosNotSent(long id, int rows, CancellationToken token = default);

    /// <summary>
    /// Update url external on hosting
    /// </summary>
    /// <param name="mediaId">Id media database</param>
    /// <param name="urlExternal">New url</param>
    Task UpdateSend(long mediaId, string urlExternal);

    /// <summary>
    /// Delete media from database
    /// </summary>
    /// <param name="id">Media id</param>
    Task Delete(long id);

    /// <summary>
    /// Update date publish
    /// </summary>
    /// <param name="mediaIds"></param>
    /// <param name="token"></param>
    Task UpdateDatePublish(long[] mediaIds, CancellationToken token = default);
    
    /// <summary>
    /// Get not sending medias
    /// </summary>
    /// <param name="id">Last id</param>
    /// <param name="rowsMessages">Count of rows</param>
    /// <param name="stoppingToken"></param>
    Task<List<Data.Media>> GetNotSent(long id, int rowsMessages, CancellationToken stoppingToken);
}
