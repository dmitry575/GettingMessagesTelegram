namespace GettingMessagesTelegram.Drivers.PostImage
{
    /// <summary>
    /// Publish photos to hosting
    /// </summary>
    public interface IPublishMediaService
    {
        /// <summary>
        /// Get not published medias and send them to free hosting
        /// </summary>
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
