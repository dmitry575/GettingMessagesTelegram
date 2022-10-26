namespace GettingMessagesTelegram.Services;

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
    /// Update url external on hosting
    /// </summary>
    /// <param name="mediaId">Id mediain database</param>
    /// <param name="urlExternal">New url</param>
    Task UpdateSend(long mediaId, string urlExternal);

    /// <summary>
    /// Delete media from database
    /// </summary>
    /// <param name="id">Media id</param>
    Task Delete(long id);
}
