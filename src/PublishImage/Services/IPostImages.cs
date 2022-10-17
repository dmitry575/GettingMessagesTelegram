using GettingMessagesTelegram.Data;
using PublishImage.Models;

namespace PublishImage.Services;

/// <summary>
/// Send image to  PostImages.org
/// </summary>
public interface IPostImages
{
    Task<PostImagesResult> Send(Media media);
}
