
using GettingMessagesTelegram.Drivers.PostImage.Models;
using GettingMessagesTelegram.Drivers.Youtube.Models;

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
        /// <param name="title">Title of video</param>
        /// <param name="description">Description of video</param>
        /// <param name="fileName">Full file path</param>
        /// <param name="cancellation"></param>
        Task<UploadResult> UploadAsync(string title, string description, string fileName, CancellationToken cancellation = default);
    }
}
