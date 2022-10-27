namespace GettingMessagesTelegram.Drivers.PostImage.Models;

/// <summary>
/// Result of sending image to  PostImages.org
/// </summary>
public class PostImagesResult
{
    /// <summary>
    /// Url for image
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Successfully send image to hosting
    /// </summary>
    public bool Success { get; set; }
}
