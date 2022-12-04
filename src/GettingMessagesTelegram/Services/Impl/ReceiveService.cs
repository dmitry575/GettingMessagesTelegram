using GettingMessagesTelegram.Config;
using GettingMessagesTelegram.Enums;
using GettingMessagesTelegram.Process;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
    private const int MaxRowsInRequest = 50;

    /// <summary>
    /// Max exceptions on the one page
    /// </summary>
    private const int MaxError = 5;

    static readonly Dictionary<long, User> Users = new();
    static readonly Dictionary<long, ChatBase> Chats = new();

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
        
        var page = 0;
        var pageError = 0;
        var countError = 0;
        try
        {
            foreach (var channel in _channelsConfig)
            {
                var channelSql = await _channelsService.CheckAdd(channel.Id, channel.HashAccess, channel.Name);

                var peerChanel = new InputPeerChannel(channel.Id, channel.HashAccess);
                _logger.LogInformation($"reading messages from channel: {channel.Id}");
                var lastDate = DateTime.MaxValue;
                var needBreak = false;
                page = 0;
                pageError = 0;
                countError = 0;

                // last date is ok or need finished to parsing messages
                while (_maxDate < lastDate && !needBreak)
                {
                    try
                    {
                        var messages = await _clientTelegram.Messages_GetHistory(peerChanel, add_offset: page, limit: MaxRowsInRequest);

                        if (messages == null || messages.Count <= 0)
                        {
                            _logger.LogInformation($"messages is ended");
                            break;
                        }

                        foreach (var message in messages.Messages)
                        {
                            _logger.LogInformation($"new message: {message.ID} - start");

                            var (status, _) = await _messageProcess.Processing(channelSql, message, cancellationToken);
                            if (status == StatusProcess.Break)
                            {
                                // set flag what this last circle
                                needBreak = true;
                            }

                            if (message != null)
                            {
                                lastDate = message.Date;
                                _logger.LogInformation($"new message: {message.ID} - finished, date: {lastDate.ToString("yyyy.MM.dd")}");
                            }
                            await Task.Delay(2500, cancellationToken);
                        }

                        if (messages.Messages?.Length < MaxRowsInRequest)
                        {
                            _logger.LogInformation(
                                $"get lower than need messages {messages.Messages.Length} - finished, date: {lastDate.ToString("yyyy.MM.dd")}");
                            break;
                        }

                        page += MaxRowsInRequest;
                    }
                    catch (RpcException e)
                    {
                        _logger.LogError($"error get messages and processing: {channel.Id}, page: {page}, {e.Source}, {e.TargetSite}, {e}");
                        if (e.Code == 401)
                        {
                            await _clientTelegram.LoginUserIfNeeded();
                            continue;
                        }
                        if (isError())
                        {
                            _logger.LogError($"too many errors: {channel.Id}, page: {page}");
                            break;
                        }
                    }
                    catch (ApplicationException e)
                    {
                        _logger.LogError($"error get messages and processing: {channel.Id}, page: {page}, {e.Source}, {e.TargetSite}, {e}");
                        await _clientTelegram.ConnectAsync();

                        if (isError())
                        {
                            _logger.LogError($"too many errors: {channel.Id}, page: {page}");
                            break;

                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"error get messages and processing their: {channel.Id}, page: {page}, {e}");
                        if (isError())
                        {
                            _logger.LogError($"too many errors: {channel.Id}, page: {page}");
                            break;
                        }
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

        bool isError()
        {
            if (pageError != page)
            {
                pageError = page;
                countError = 1;
            }
            else
            {
                countError++;
            }
            return countError > MaxError;
        }
    }

    public void SubscribeToEvents()
    {
        _clientTelegram.OnUpdate += ListenUpdate;
    }


    private async Task ListenUpdate(IObject arg)
    {
        if (arg is not UpdatesBase updates)
        {
            _logger.LogWarning($"received not UpdatesBase event: {arg.GetType()}");
            return;
        }
        foreach (var update in updates.UpdateList)
            switch (update)
            {
                case UpdateNewMessage unm: await ProcessEventMessage(unm.message); break;
                case UpdateEditMessage uem: await ProcessEventMessage(uem.message); break;
                case UpdateUserStatus: break;
                default: _logger.LogInformation($"handle a unknown type message type: {update.GetType().Name}"); break; 
            }
    }

    private async Task ProcessEventMessage(MessageBase message)
    {
        try
        {
            var channel = _channelsConfig.FirstOrDefault(x => x.Id == message.Peer.ID);
            if (channel == null)
            {
                _logger.LogInformation($"message from unknown channel: {message.Peer.ID}, {message.Date}");
                return;
            }
            var channelSql = await _channelsService.CheckAdd(channel.Id, channel.HashAccess, channel.Name);
            if (channelSql == null)
            {
                _logger.LogInformation($"message from unknown channel: {message.Peer.ID}, {message.Date}");
                return;
            }

            var (status, data) = await _messageProcess.Processing(channelSql, message);
            _logger.LogInformation($"message added or update: {status}, new id: {data?.Id}, base id: {data?.BaseId}");


            switch (message)
            {
                case TL.MessageService ms:
                    _logger.LogInformation($"MessageService: from {ms.from_id} to ms.peer_id: [{ms.action.GetType().Name}]");
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"MessageService: message {message.ID}  failed processing, {e}");
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

    public void Dispose()
    {
        _clientTelegram?.Dispose();
    }
}
