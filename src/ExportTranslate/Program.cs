using System.Diagnostics;
using ExportTranslate.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GettingMessagesTelegram.Services;
using GettingMessagesTelegram.Services.Impl;
using GettingMessagesTelegram.DataAccess;
using Microsoft.EntityFrameworkCore;
using GettingMessagesTelegram.Drivers.Translates;
using GettingMessagesTelegram.Drivers.Translates.Imp;
using GettingMessagesTelegram.Drivers.Translates.Config;

System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

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
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IMessageTranslateService, MessageTranslateService>();
        services.AddSingleton<ICommentTranslateService, CommentTranslateService>();
            
        services.AddSingleton<IExport, Export>();
        services.AddHostedService<ExportService>();
    })
    .Build();


Console.WriteLine($"Version: {fvi.FileVersion}");
Console.WriteLine($"Starting the export files for translating");
await host.RunAsync();