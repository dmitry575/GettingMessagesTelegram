using System.Diagnostics;
using GettingMessagesTelegram.Config;
using GettingMessagesTelegram.DI;
using Microsoft.Extensions.Configuration;
using GettingMessagesTelegram.Services;
using GettingMessagesTelegram.Services.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WTelegram;

var cancellationTokenSource = new CancellationTokenSource();
var token = cancellationTokenSource.Token;

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

        services.AddHostedService<Worker>();

        services.AddSingleton(sp =>
        {
            var telegramConfig = sp.GetService<IOptions<TelegramConfig>>();
            var readTelegramConfig = new ReadTelegramConfig(telegramConfig?.Value);
            return new Client(readTelegramConfig.Read);
        });
    })
    .Build();


Console.WriteLine($"Version: {fvi.FileVersion}");
Console.WriteLine("Starting the reading message from Telegram");
await host.RunAsync();
