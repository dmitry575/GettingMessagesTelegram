using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GettingMessagesTelegram.Services;
using GettingMessagesTelegram.Services.Impl;
using GettingMessagesTelegram.DataAccess;
using ImportTranslate.Services;
using Microsoft.EntityFrameworkCore;
using GettingMessagesTelegram.Drivers.Translates;
using GettingMessagesTelegram.Drivers.Translates.Imp;
using GettingMessagesTelegram.Drivers.Translates.Config;

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
        services.Configure<TranslatesConfig>(configuration.GetSection("Translates"));

        services.AddSingleton(configuration);

        services.AddScoped(c => new HttpClient(new HttpClientHandler()));
        services.AddSingleton<IMediaService, MediaService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IImport, Import>();
        services.AddHostedService<ImportService>();
    })
    .Build();


Console.WriteLine($"Version: {fvi.FileVersion}");
Console.WriteLine($"Starting the import files for translating");
await host.RunAsync();