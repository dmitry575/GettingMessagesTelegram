﻿using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublishImage.Services;
using GettingMessagesTelegram.Services;
using GettingMessagesTelegram.Services.Impl;
using GettingMessagesTelegram.DataAccess;
using GettingMessagesTelegram.Drivers.PostImage;
using GettingMessagesTelegram.Drivers.PostImage.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        services.AddLogging(configure =>
        {
            configure.SetMinimumLevel(LogLevel.Information);
            configure.AddLog4Net();
            configure.AddConsole();
        });
        services.AddSingleton(configuration);

        services.AddScoped(c => new HttpClient(new HttpClientHandler()));
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IMediaService, MediaService>();
        services.AddSingleton<IPublishMediaService, PublishMediaService>();
        services.AddSingleton<IPostImages, PostImages>();
        
        services.AddHostedService<PublishService>();
    })
    .Build();


Console.WriteLine($"Version: {fvi.FileVersion}");
Console.WriteLine($"Starting the publish images to Postimage.org");
await host.RunAsync();
