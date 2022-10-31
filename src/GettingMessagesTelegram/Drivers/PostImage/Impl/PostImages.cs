using System.Globalization;
using System.Text.RegularExpressions;
using GettingMessagesTelegram.Drivers.PostImage.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PublishImage.Helpers;

namespace GettingMessagesTelegram.Drivers.PostImage.Impl;

public class PostImages : IPostImages
{
    private const string BaseUrl = "https://postimages.org";
    private const string PostImageUrl = "/json/rr";

    private readonly HttpClient _httpClient;
    private readonly ILogger<PostImages> _logger;
    private string _token;

    public PostImages(HttpClient httpClient, ILogger<PostImages> logger)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Referer", BaseUrl);
        
        _httpClient.BaseAddress = new Uri(BaseUrl);

        _logger = logger;
    }

    public async Task<UploadResult> SendAsync(Data.Media media)
    {
        var result = new UploadResult { Success = false };

        // check file
        if (!File.Exists(media.LocalPath))
        {
            _logger.LogWarning($"images not exists: {media.LocalPath}");
            return result;
        }

        var fileInfo = new FileInfo(media.LocalPath);
        if (fileInfo.Length <= 0)
        {
            _logger.LogWarning($"images {media.LocalPath} file length is empty");
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

        if (!string.IsNullOrEmpty(url))
        {
            return new UploadResult { Success = true, Url = url };
        }

        return result;
    }

    /// <summary>
    /// Send file to hosting
    /// </summary>
    /// <param name="media">Information about file</param>
    private async Task<string> SendRequest(Data.Media media)
    {
        using var content =
            new MultipartFormDataContent("----" + DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));

        content.Add(new StreamContent(new FileStream(media.LocalPath, FileMode.Open)), "file", media.BaseId + ".jpg");
        SetData(content,
            new UploadRequest { Token = _token, UploadSession = SessionHelper.GetSession(), Gallery = string.Empty });

        using var message = await _httpClient.PostAsync(BaseUrl+PostImageUrl, content);
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

    /// <summary>
    /// Parsing response after upload and get direct url to image
    /// </summary>
    private async Task<string> GetResponse(UploadResponse response)
    {
        try
        {
            var imagesUls = await _httpClient.GetAsync(response.Url);
            var html = await imagesUls.Content.ReadAsStringAsync();
            HtmlDocument hap = new HtmlDocument();
            hap.LoadHtml(html);
            HtmlNode input = hap.DocumentNode.SelectSingleNode("//input[@id='code_direct']");

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

    /// <summary>
    /// Setting data to form
    /// </summary>
    private static void SetData(MultipartFormDataContent content, UploadRequest request)
    {
        content.Add(new StringContent(request.Token), "token");
        content.Add(new StringContent(request.UploadSession), "upload_session");
        content.Add(new StringContent(request.Numfiles.ToString()), "numfiles");
        content.Add(new StringContent(request.Optsize.ToString()), "optsize");
        content.Add(new StringContent(request.Gallery), "gallery");
        content.Add(new StringContent(request.Expire.ToString()), "expire");
        content.Add(new StringContent(request.Source), "source");
        
    }

    /// <summary>
    /// Get token for working
    /// </summary>
    private async Task<string> GetApiToken()
    {
        try
        {
            var response = await _httpClient.GetAsync("/");
            var html = await response.Content.ReadAsStringAsync();
            var matches = Regex.Matches(html, "\"token\",\"(\\w*)\"");
            for (var i = 0; i < matches.Count; i++)
            {
                if (matches[i].Success)
                {
                    return matches[i].Groups[1].Value;
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"get token failed: {e}");
        }

        return string.Empty;
    }
}
