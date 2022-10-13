using GettingMessagesTelegram.Config;
using GettingMessagesTelegram.Enums;
using GettingMessagesTelegram.Process;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TL;
using WTelegram;
using Channel = TL.Channel;

namespace GettingMessagesTelegram.Services.Impl;

public class ReceiveService : IReceiveService
{
    /// <summary>
    /// Client for Telegram
    /// </summary>
    private readonly Client _clientTelegram;

    private readonly ILogger<ReceiveService> _logger;

    /// <summary>
    /// Configurationn for getting messages for channels
    /// </summary>
    private readonly ChannelsConfig _channelsConfig;

    /// <summary>
    /// Min date for parsing messages
    /// </summary>
    private readonly DateTime _maxDate;

    private readonly IChannelsService _channelsService;

    private readonly IMessageProcess _messageProcess;

    /// <summary>
    /// Max of rows in one requests for pagination
    /// </summary>
    private const int MaxRowsInRequest = 30;

    public ReceiveService(Client clientTelegram, ILogger<ReceiveService> logger,
        IOptions<ChannelsConfig> channelsConfig, IChannelsService channelsService, IMessageProcess messageProcess)
    {
        _clientTelegram = clientTelegram;
        _logger = logger;
        _channelsService = channelsService;
        _messageProcess = messageProcess;
        _channelsConfig = channelsConfig.Value;
        _maxDate = DateTime.UtcNow.AddYears(-1);
    }

    public async Task WorkAsync(CancellationToken cancellationToken)
    {
        var me = await _clientTelegram.LoginUserIfNeeded();
        _logger.LogInformation($"Loggin by: {me.first_name}");
        try
        {
            foreach (var channel in _channelsConfig)
            {
                var channelSql = await _channelsService.CheckAdd(channel.Id, channel.HashAccess, channel.Name);

                var peerChanel = new InputPeerChannel(channel.Id, channel.HashAccess);
                _logger.LogInformation($"reading messages from channel: {channel.Id}");
                var lastDate = DateTime.MaxValue;
                var needBreak = false;
                var page = 0;

                // last date is ok or need finished to parsing messages
                while (_maxDate < lastDate && !needBreak)
                {
                    try
                    {
                        var messages =
                            await _clientTelegram.Messages_GetHistory(peerChanel, add_offset: page,
                                limit: MaxRowsInRequest);
                        foreach (var message in messages.Messages)
                        {
                            _logger.LogInformation($"new message: {message.ID} - start");

                            var (status, data) = await _messageProcess.Processing(channelSql, message, cancellationToken);
                            if (status == StatusProcess.Break)
                            {
                                // set flag what this last circkle
                                needBreak = true;
                            }

                            await _downloadService.DownloadAsync(data);
                            _clientTelegram.DownloadFileAsync()
                            if (message != null)
                            {
                                lastDate = message.Date;
                                _logger.LogInformation($"new message: {message.ID} - finished, date: {lastDate.ToString("yyyy.MM.dd")}");
                            }
                            Thread.Sleep(2500);
                        }

                        if (messages.Messages?.Length < MaxRowsInRequest)
                        {
                            _logger.LogInformation(
                                $"get lower than need messages {messages.Messages.Length} - finished, date: {lastDate.ToString("yyyy.MM.dd")}");
                            break;
                        }

                        page += MaxRowsInRequest;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"error get messages and processing their: {channel.Id}, page: {page}, {e}");
                    }
                }

                if (lastDate < _maxDate)
                {
                    _logger.LogInformation(
                        $"parsing too many messages last date: {lastDate.ToString("yyyy.MM.dd")}, max date: {_maxDate.ToString("yyyy.MM.dd")}");
                }

                _logger.LogInformation($"reading messages was finished from channel: {channel.Id}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError("reading messages failed");
            Console.WriteLine(e);
        }
    }


    private async Task PrintAllChats()
    {
        var chats = await _clientTelegram.Messages_GetAllChats();
        Console.WriteLine("This user has joined the following:");
        foreach (var (id, chat) in chats.chats)
            switch (chat) // example of downcasting to their real classes:
            {
                case Chat basicChat when basicChat.IsActive:
                    Console.WriteLine(
                        $"{id}:  Basic chat: {basicChat.title} with {basicChat.participants_count} members");
                    break;
                case Channel group when group.IsGroup:
                    Console.WriteLine($"{id}: Group {group.username}: {group.title}");
                    break;
                case Channel channel:
                    Console.WriteLine($"{id}: Channel {channel.username}: {channel.title}, {channel.access_hash}");
                    break;
            }
    }
}
