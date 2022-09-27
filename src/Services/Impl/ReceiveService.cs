using GettingMessagesTelegram.Config;
using GettingMessagesTelegram.Extensions;
using GettingMessagesTelegram.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TL;
using WTelegram;

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
    private readonly IMessageService _messageService;

    private enum StatusProcess
    {
        Ok,
        Failed,
        Break
    }

    public ReceiveService(Client clientTelegram, ILogger<ReceiveService> logger,
        IOptions<ChannelsConfig> channelsConfig, IChannelsService channelsService, IMessageService messageService)
    {
        _clientTelegram = clientTelegram;
        _logger = logger;
        _channelsService = channelsService;
        _messageService = messageService;
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

                // last date is ok or need finished to parsing messages
                while (_maxDate < lastDate && !needBreak)
                {
                    var messages =
                        await _clientTelegram.Messages_GetHistory(peerChanel, offset_date: lastDate, limit: 100);
                    foreach (var message in messages.Messages)
                    {
                        _logger.LogInformation($"new message: {message.ID} - start");
                        var (status, messageData) = await ProcessMessage(channelSql.Id, message);
                        if (status == StatusProcess.Break)
                        {
                            // set flag what this last circkle
                            needBreak = true;
                        }

                        var comments = await _clientTelegram.Messages_GetDiscussionMessage(peerChanel, message.ID);
                        // add or update comments
                        ProcessComments(messageData, comments);

                        var updates = await _messageService.ReplaceAsync(messageData, cancellationToken);

                        lastDate = message.Date;
                        _logger.LogInformation(
                            $"new message: {message.ID} - finished, date: {lastDate.ToString("yyyy.MM.dd")}, updates: {updates}");
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

    /// <summary>
    /// Processing comments for message
    /// </summary>
    /// <param name="message">MEssage from our database with comments</param>
    /// <param name="comments">Comments from telegram</param>
    private void ProcessComments(Data.Message message, Messages_DiscussionMessage comments)
    {
        foreach (var comment in comments.messages)
        {
            var c = message.Comments.FirstOrDefault(x => x.BaseId == comment.ID);
            if (c is null)
            {
                c = (comment as Message)?.MapToComment();
                if (c != null)
                {
                    message.Comments.Add(c);
                }
            }
        }

        if (message.Comments != null)
        {
            message.CommentCount = message.Comments.Count;
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

    private async Task<(StatusProcess, Data.Message)> ProcessMessage(long channelId, MessageBase message)
    {
        if (message is Message m)
        {
            var (exists, messageData) = await MessageExists(channelId, m.ID);
            if (exists)
            {
                _logger.LogInformation("found last exists message: " + m.ID + "\t" + m.message + "\t" + m.post_author);
                messageData.ViewCount = m.views;
            }
            else
            {
                // create a new message to database
                messageData = m.Map();
                messageData.ChannelId = channelId;
            }

            _logger.LogInformation(m.ID + "\t" + m.post_author);

            //var messageData = await SaveMessage(m);

            var link = UrlHelper.GetTmeUrl(m.message);
            if (!string.IsNullOrEmpty(link))
            {
                _logger.LogInformation($"found the link {link} in the message {m.ID}");
                // var linkInfo = await _clientTelegram.Help_GetDeepLinkInfo(link);
                // _logger.LogInformation(linkInfo?.message);
                // var path = link.Replace("https://t.me/", "");
                // linkInfo = await _clientTelegram.Help_GetDeepLinkInfo(path);
                // _logger.LogInformation(linkInfo?.message);
                // var ll = await _clientTelegram.Help_GetRecentMeUrls(link);
                // //_logger.LogInformation(ll?.urls.Length);
                // //_clientTelegram.Get
                // _clientTelegram.GetFullChat(new InputPeerChannel())
            }

            return (exists ? StatusProcess.Break : StatusProcess.Ok, messageData);
        }

        return (StatusProcess.Failed, null);
    }

    /// <summary>
    /// If exists messages
    /// </summary>
    private async Task<(bool, Data.Message)> MessageExists(long channelId, long messageId)
    {
        var message = await _messageService.GetByBaseId(channelId, messageId);
        return (message is null, message);
    }
}
