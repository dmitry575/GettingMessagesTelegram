using GettingMessagesTelegram.DataAccess;

namespace GettingMessagesTelegram.Services;

public interface IMediaService
{
    /// <summary>
    /// Create or update media data of message
    /// </summary>
    /// <param name="media">Information of photo or video</param>
    Task UpdateOrCreate(Media media);
}
