using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Enums;
using GettingMessagesTelegram.Extensions;
using GettingMessagesTelegram.Helpers;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Logging;
using TL;
using WTelegram;
using Message = GettingMessagesTelegram.Data.Message;

namespace GettingMessagesTelegram.Process.Impl;

public class MessageProcess : IMessageProcess
{
    private readonly ILogger<MessageProcess> _logger;
    private readonly IMessageService _messageService;

    /// <summary>
    /// Max of rows in one requests for pagination
    /// </summary>
    private const int MaxRowsComments = 100;

    /// <summary>
    /// Client for Telegram
    /// </summary>
    private readonly Client _clientTelegram;

    public MessageProcess(IMessageService messageService, ILogger<MessageProcess> logger, Client clientTelegram)
    {
        _messageService = messageService;
        _logger = logger;
        _clientTelegram = clientTelegram;
    }

    public async Task<(StatusProcess, Message)> Processing(Data.Channel channel, MessageBase message,
        CancellationToken cancellationToken = default)
    {
        var (status, messageData) = await WorkMessage(channel.Id, message);
        if (status == StatusProcess.Break)
        {
            // set flag what this last circkle
            return (status, messageData);
        }

        var peerChanel = new InputPeerChannel(channel.BaseId, channel.HashAccess);
        // add or update comments
        await ProcessComments(messageData, peerChanel, message.ID);

        var updates = await _messageService.ReplaceAsync(messageData, cancellationToken);

        if (message is TL.Message m)
        {
            if (m.media != null)
            {
                if ((m.flags & TL.Message.Flags.has_media) != 0)
                {
                    
                }
                
            }
        }
        
        _logger.LogInformation(
            $"updated message channel id: {channel.BaseId}, message id: {message.ID}, updated: {updates}");

        return (StatusProcess.Ok, messageData);
    }

    private async Task<(StatusProcess status, Message messageData)> WorkMessage(long channelId, MessageBase message)
    {
        if (message is not TL.Message m)
        {
            _logger.LogWarning("message telegram has not ");
            return (StatusProcess.Failed, null);
        }

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

    /// <summary>
    /// Processing comments for message
    /// </summary>
    /// <param name="message">MEssage from our database with comments</param>
    /// <param name="peerChanel">Data of channel</param>
    /// <param name="telegramMessageId">Telegram message id</param>
    private async Task ProcessComments(Data.Message message, InputPeerChannel peerChanel, Int32 telegramMessageId)
    {
        message.Comments ??= new List<Comment>();

        var page = 0;
        while (true)
        {
            try
            {
                var comments =
                    await _clientTelegram.Messages_GetReplies(peerChanel, telegramMessageId, limit: MaxRowsComments,
                        add_offset: page);

                foreach (var comment in comments.Messages)
                {
                    var c = message.Comments?.FirstOrDefault(x => x.BaseId == comment.ID);
                    if (c is null)
                    {
                        c = (comment as TL.Message)?.MapToComment();
                        if (c != null)
                        {
                            message.Comments.Add(c);
                        }
                    }
                }


                if (comments.Messages.Length < MaxRowsComments)
                    break;

                page += MaxRowsComments;
            }
            catch (RpcException e)
            {
                _logger.LogError(
                    $"get comments for message: {telegramMessageId} failed, page: {page}, code: {e.Code}, {e}");
                break;
            }
            catch (Exception e)
            {
                _logger.LogError($"get comments for message: {telegramMessageId} failed, page: {page}, {e}");
                break;
            }
        }

        message.CommentCount = message.Comments.Count;
    }

    /// <summary>
    /// If exists messages
    /// </summary>
    private async Task<(bool, Message)> MessageExists(long channelId, long messageId)
    {
        var message = await _messageService.GetByBaseId(channelId, messageId);
        return (message is not null, message);
    }
}
