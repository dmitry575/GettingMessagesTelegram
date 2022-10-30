
namespace GettingMessagesTelegram.Drivers.Youtube
{
    /// <summary>
    /// Upload video to youtube
    /// </summary>
    public interface IYouTubeUploader
    {
        /// <summary>
        /// Upload one file to youtube
        /// </summary>
        /// <param name="fileName">Full file path</param>
        /// <param name="cancellation"></param>
        Task<bool> UploadAsync(string fileName, CancellationToken cancellation = default);
    }
}
