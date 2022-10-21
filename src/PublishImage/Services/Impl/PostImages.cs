using GettingMessagesTelegram.Data;
using PublishImage.Models;

namespace PublishImage.Services.Impl;

public class PostImages : IPostImages
{
    private readonly HttpClient _httpClient;
    public PostImages(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public Task<PostImagesResult> SendAsync(Media media)
    {
        return null;
    }
}
