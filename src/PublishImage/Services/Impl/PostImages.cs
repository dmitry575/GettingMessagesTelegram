using GettingMessagesTelegram.Data;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PublishImage.Models;
using System.Globalization;

namespace PublishImage.Services.Impl;

public class PostImages : IPostImages
{
    private const string BaseUrl = "https://postimages.org/";
    private const string ApiUrl = "/login/api";
    private const string PostImageUrl = "/json/rr";

    private readonly HttpClient _httpClient;
    private readonly ILogger<PublishService> _logger;
    private string _token;

    public PostImages(HttpClient httpClient, ILogger<PublishService> logger)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.BaseAddress = new Uri(BaseUrl);


        _logger = logger;
    }
    public async Task<PostImagesResult> SendAsync(Media media)
    {
        var result = new PostImagesResult { Success = false };

        // check file
        if (!File.Exists(media.LocalPath))
        {
            _logger.LogWarning($"images not exists: {media.LocalPath}");
            return result;
        }

        // get token
        if (string.IsNullOrEmpty(_token))
        {
            _token = await GetApiToken();
            if (string.IsNullOrEmpty(_token))
            {
                _logger.LogWarning("token is empty");
                return result;
            }
        }

        var url = await SendRequest(media);

        if (string.IsNullOrEmpty(url))
        {
            return new PostImagesResult { Success = true, Url = url };
        }


        return result;
    }

    /// <summary>
    /// Send file to hosting
    /// </summary>
    /// <param name="media">Information about file</param>
    private async Task<string> SendRequest(Media media)
    {
        using var content = new MultipartFormDataContent("----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));

        content.Add(new StreamContent(new FileStream(media.LocalPath, FileMode.Open)), "file", media.FileName + ".jpg");
        SetData(content, new UploadRequest { Token = _token });

        using var message = await _httpClient.PostAsync(PostImageUrl, content);
        _logger.LogInformation($"response send file to hosting: {media.LocalPath}, message: {media.MessageId}");

        var json = await message.Content.ReadAsStringAsync();
        _logger.LogInformation($"response send file to hosting: {json}");

        var response = JsonConvert.DeserializeObject<UploadResponse>(json);
        if (response?.Status == "OK")
        {
            return await GetResponse(response);
        }

        return string.Empty;
    }

    private async Task<string> GetResponse(UploadResponse response)
    {
        try
        {
            var imagesUls = await _httpClient.GetAsync(response.Url);
            var html = await imagesUls.Content.ReadAsStringAsync();
            HtmlDocument hap = new HtmlDocument();
            hap.LoadHtml(html);
            HtmlNode input = hap.DocumentNode.SelectSingleNode("//input[@id='code_html']");

            if (input != null)
            {
                return input.GetAttributeValue("value", string.Empty);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"get token failed: {e}");
        }
        return string.Empty;
    }

    private static void SetData(MultipartFormDataContent content, UploadRequest request)
    {
        content.Add(new StringContent("token"), request.Token);
        content.Add(new StringContent("upload_session"), request.UploadSession);
        content.Add(new StringContent("numfiles"), request.Numfiles.ToString());
        content.Add(new StringContent("optsize"), request.Optsize.ToString());
        content.Add(new StringContent("gallery"), request.Gallery);
        content.Add(new StringContent("\""), request.Expire.ToString());
    }

    /// <summary>
    /// Get token for working
    /// </summary>
    private async Task<string> GetApiToken()
    {
        try
        {
            var response = await _httpClient.GetAsync(ApiUrl);
            var html = await response.Content.ReadAsStringAsync();
            HtmlDocument hap = new HtmlDocument();
            hap.LoadHtml(html);
            HtmlNode input = hap.DocumentNode.SelectSingleNode("//input[@id='api_key']");

            if (input != null)
            {
                return input.GetAttributeValue("value", string.Empty);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"get token failed: {e}");
        }
        return string.Empty;
    }
}
