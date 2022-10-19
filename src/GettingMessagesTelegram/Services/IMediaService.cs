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
}
