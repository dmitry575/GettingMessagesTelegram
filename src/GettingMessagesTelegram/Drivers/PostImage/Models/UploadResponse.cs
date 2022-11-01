
namespace GettingMessagesTelegram.Drivers.PostImage.Models;

/// <summary>
/// Response after upload image
/// </summary>
public class UploadResponse
{
    /// <summary>
    /// Status request
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Url result
    /// </summary>
    public string Url { get; set; }
}