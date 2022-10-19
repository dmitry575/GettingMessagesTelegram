﻿using System.Diagnostics;
using GettingMessagesTelegram.Config;
using GettingMessagesTelegram.DI;
using GettingMessagesTelegram.Services;
using GettingMessagesTelegram.Services.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PublishImage.Services;

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
        services.Configure(configuration);

        services.AddHostedService<PublishService>();
    })
    .Build();


Console.WriteLine($"Version: {fvi.FileVersion}");
Console.WriteLine($"Starting the publish images to Postimage.org");
await host.RunAsync();