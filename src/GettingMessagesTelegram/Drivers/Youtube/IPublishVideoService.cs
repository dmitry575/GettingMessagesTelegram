namespace GettingMessagesTelegram.Drivers.Youtube
{
    /// <summary>
    /// Get all do not sending video and send all of them
    /// </summary>
    public interface IPublishVideoService
    {
        /// <summary>
        /// Get not published videos and send them to free hosting
        /// </summary>
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
