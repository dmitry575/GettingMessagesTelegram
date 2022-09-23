using System.Configuration;
using GettingMessagesTelegram.DataAccess;
using GettingMessagesTelegram.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GettingMessagesTelegram.DI;

public static class ConfigureServices
{
    public static IServiceCollection Configure(this IServiceCollection services, IConfiguration configuration)
    {
        // get connection string to database
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MessagesContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton(configuration);
        services.AddSingleton<IChannelsService, ChannelsService>();

        services.AddLogging(configure =>
        {
            configure.SetMinimumLevel(LogLevel.Trace);
            configure.AddLog4Net();
            configure.AddConsole();
        });

        services.AddLogging();
        return services;
    }
}
