namespace GettingMessagesTelegram.Services;

public interface IDownloadService
{
    /// <summary>
    /// Download all media files
    /// </summary>
    /// <param name="message">message</param>
    Task DownloadAsync(Data.Message message);
}
