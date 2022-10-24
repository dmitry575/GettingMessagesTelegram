using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GettingMessagesTelegram.Data;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PublishImage.Services;
using PublishImage.Services.Impl;

namespace PublishImage.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task SendAsync()
    {
        var httpClient = new HttpClient(new HttpClientHandler());
        var postImages = new PostImages(httpClient, Mock.Of<ILogger<PublishService>>());

        var media = new Media()
        {
            LocalPath = "data\\test.jpeg"
        };
        var t = await postImages.SendAsync(media);
        t.Success.Should().BeTrue();
        t.Url.Should().NotBeNullOrEmpty();
    }
}
