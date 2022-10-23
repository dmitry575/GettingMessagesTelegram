﻿using GettingMessagesTelegram.Data;
using PublishImage.Models;

namespace PublishImage.Services;

/// <summary>
/// Send image to  PostImages.org
/// </summary>
public interface IPostImages
{
    /// <summary>
    /// Sending file to server
    /// </summary>
    /// <param name="media">Information about file</param>
    Task<PostImagesResult> SendAsync(Media media);
}
