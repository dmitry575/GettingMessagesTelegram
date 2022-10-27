using GettingMessagesTelegram.Drivers.PostImage.Models;

namespace GettingMessagesTelegram.Drivers.PostImage;

/// <summary>
/// Send image to  PostImages.org
/// </summary>
public interface IPostImages
{
    /// <summary>
    /// Sending file to server
    /// </summary>
    /// <param name="media">Information about file</param>
    Task<PostImagesResult> SendAsync(Data.Media media);
}
