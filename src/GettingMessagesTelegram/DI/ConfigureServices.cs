using GettingMessagesTelegram.Config;
using GettingMessagesTelegram.DataAccess;
using GettingMessagesTelegram.Drivers.PostImage.Impl;
using GettingMessagesTelegram.Drivers.PostImage;
using GettingMessagesTelegram.Drivers.Translates.Config;
using GettingMessagesTelegram.Media;
using GettingMessagesTelegram.Media.Impl;
using GettingMessagesTelegram.Process;
using GettingMessagesTelegram.Process.Impl;
using GettingMessagesTelegram.Services;
using GettingMessagesTelegram.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GettingMessagesTelegram.Drivers.Youtube.Config;
using GettingMessagesTelegram.Drivers.Youtube;
using GettingMessagesTelegram.Drivers.Youtube.Impl;

namespace GettingMessagesTelegram.DI;

public static class ConfigureServices
{
    public static IServiceCollection Configure(this IServiceCollection services, IConfiguration configuration)
    {
        // get connection string to database
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MessagesContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton(configuration);
        services.AddSingleton<IReceiveService, ReceiveService>();
        services.AddSingleton<IReadTelegramConfig, ReadTelegramConfig>();
        services.AddSingleton<IChannelsService, ChannelsService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IMessageProcess, MessageProcess>();
        services.AddSingleton<IMediaService, MediaService>();
        services.AddSingleton<IMediaCreator, MediaCreator>();
        services.AddSingleton<IPostImages, PostImages>();
        services.AddScoped<IYouTubeUploader, YouTubeUploader>();

        services.AddLogging(configure =>
        {
            configure.AddLog4Net();
            configure.AddConsole();
        });

        services.AddLogging();

        services.Configure<TelegramConfig>(configuration.GetSection("Telegram"));
        services.Configure<ChannelsConfig>(configuration.GetSection("Channels"));
        services.Configure<DownloadConfig>(configuration.GetSection("Download"));
        services.Configure<TranslatesConfig>(configuration.GetSection("Translates"));
        services.Configure<YoutubeConfig>(configuration.GetSection("Youtube"));
        
        return services;
    }
}
