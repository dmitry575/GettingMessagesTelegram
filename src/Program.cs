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
        services.
        var client = new WTelegram.Client(Config);
        services.AddSingleton(client);

        services.AddHostedService<Worker>();
    })
    .Build();


Console.WriteLine($"Version: {fvi.FileVersion}");
// var processes = new Processes(cancellationTokenSource);
// AppDomain.CurrentDomain.ProcessExit += processes.ProcessExit;
//
// using var client = new WTelegram.Client(Config);
// var channelService = new ChannelsService(client);

//await channelService.WorkAsync(token);
await host.RunAsync();

static string Config(string what)
{
    switch (what)
    {
        case "api_id": return "71522";
        case "api_hash": return "9a871fe9b5c3abad4786d6ea693b0228";
        case "phone_number": return "+79172552240";
        case "verification_code":
            Console.Write("Code: ");
            return Console.ReadLine();
        default: return null;
    }
}
