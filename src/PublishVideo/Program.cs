using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GettingMessagesTelegram.Services;
using GettingMessagesTelegram.Services.Impl;
using GettingMessagesTelegram.DataAccess;
using GettingMessagesTelegram.Drivers.PostImage;
using GettingMessagesTelegram.Drivers.PostImage.Impl;
using Microsoft.EntityFrameworkCore;
using GettingMessagesTelegram.Drivers.Youtube;
using GettingMessagesTelegram.Drivers.Youtube.Impl;
using PublishVideo.Services;

System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);


//https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

var configuration = builder.Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        // get connection string to database
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MessagesContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton(configuration);

        services.AddScoped(c => new HttpClient(new HttpClientHandler()));
        services.AddSingleton<IYouTubeUploader, YouTubeUploader>();
        services.AddSingleton<IPublishVideoService, PublishVideoService>();
        services.AddSingleton<IMediaService, MediaService>();
        services.AddSingleton<IMessageService, MessageService>();

        services.AddHostedService<PublishService>();

    })
    .Build();


Console.WriteLine($"Version: {fvi.FileVersion}");
Console.WriteLine($"Starting the publish videos to youtube.com");
await host.RunAsync();