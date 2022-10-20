namespace PublishImage.Models;

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
    /// Successufly send image to hosting
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Any error diuring the sending image
    /// </summary>
    public string ErrorMessage { get; set; }
}
