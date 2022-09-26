using System.Diagnostics;
using GettingMessagesTelegram.DI;
using Microsoft.Extensions.Configuration;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        var readerConfig = services.BuildServiceProvider().GetService<IReadTelegramConfig>();

        if (readerConfig != null)
        {
            var client = new WTelegram.Client(readerConfig.Read);
            services.AddSingleton(client);
        }
        
    })
    .Build();


Console.WriteLine($"Version: {fvi.FileVersion}");
Console.WriteLine($"Starting the reading message from Telegram");
await host.RunAsync();


