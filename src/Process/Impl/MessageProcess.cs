﻿using GettingMessagesTelegram.Config;
using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Enums;
using GettingMessagesTelegram.Extensions;
using GettingMessagesTelegram.Helpers;
using GettingMessagesTelegram.Media;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TL;
using WTelegram;
using Message = GettingMessagesTelegram.Data.Message;

namespace GettingMessagesTelegram.Process.Impl;

public class MessageProcess : IMessageProcess
{
    private readonly ILogger<MessageProcess> _logger;
    private readonly IMessageService _messageService;
    private readonly IMediaService _mediaService;

    /// <summary>
    /// Max of rows in one requests for pagination
    /// </summary>
    private const int MaxRowsComments = 100;

    /// <summary>
    /// Client for Telegram
    /// </summary>
    private readonly Client _clientTelegram;

    private readonly IMediaCreator _mediaCreator;

    private DownloadConfig _downloadConfig;


    public MessageProcess(IMessageService messageService, ILogger<MessageProcess> logger, Client clientTelegram,
        IMediaService mediaService, IMediaCreator mediaCreator, IOptions<DownloadConfig> downloadConfig)
    {
        _messageService = messageService;
        _logger = logger;
        _clientTelegram = clientTelegram;
        _mediaService = mediaService;
        _mediaCreator = mediaCreator;
        _downloadConfig = downloadConfig.Value;
    }

    public async Task<(StatusProcess, Message)> Processing(Data.Channel channel, MessageBase message,
        CancellationToken cancellationToken = default)
    {
        var (status, messageData) = await WorkMessage(channel.Id, message);
        switch (status)
        {
            case StatusProcess.Failed:
                return (status, messageData);
        }

        var peerChanel = new InputPeerChannel(channel.BaseId, channel.HashAccess);

        // add or update comments
        await ProcessComments(messageData, peerChanel, message.ID);

       await ProcessMedias(messageData, message);

        var updates = await _messageService.ReplaceAsync(messageData, cancellationToken);

        _logger.LogInformation(
            $"updated message channel id: {channel.BaseId}, message id: {message.ID}, updated: {updates}");

        return (status, messageData);
    }

    private async Task ProcessMedias(Message messageData, MessageBase message)
    {
        if (message is TL.Message { media: { } } m)
        {
            if ((m.flags & TL.Message.Flags.has_media) != 0)
            {
                _logger.LogInformation($"{m.media.GetType()}");

                // create data from message
                var media = _mediaCreator.Create(messageData.Id, m.media);
                if (media != null)
                {
                    messageData.Medias ??= new List<DataAccess.Media>();
                    media.LocalPath = await DownloadFile(m.media);

                    messageData.Medias.Add(media);
                }
            }
        }
    }

    /// <summary>
    /// Download media to the file
    /// </summary>
    /// <param name="media"></param>
    private async Task<string> DownloadFile(MessageMedia media)
    {
        if (!Directory.Exists(_downloadConfig.LocalPath))
        {
            Directory.CreateDirectory(_downloadConfig.LocalPath);
            _logger.LogInformation($"directory created: {_downloadConfig.LocalPath}");
        }
        
        if (media is MessageMediaDocument { document: Document document })
        {
            var filename = Path.Combine(_downloadConfig.LocalPath, document.ID.ToString());
            _logger.LogInformation($"starting download video file {document.ID} to {filename}");
            try
            {
                await using var fileStream = File.Create(filename);
                await _clientTelegram.DownloadFileAsync(document, fileStream);
                _logger.LogInformation($"downloaded video file {document.ID} to {filename}");
                return filename;
            }
            catch (Exception e)
            {
                _logger.LogError($"downloading file failed: id {document.ID}, {filename}, {e}");
            }
        }
        else if (media is MessageMediaPhoto { photo: Photo photo })
        {
            var filename = Path.Combine(_downloadConfig.LocalPath, photo.ID.ToString());
            _logger.LogInformation($"starting download photo file {photo.ID} to {filename}");
            try
            {
                await using var fileStream = File.Create(filename);
                await _clientTelegram.DownloadFileAsync(photo, fileStream);
                _logger.LogInformation($"downloaded photo file {photo.ID} to {filename}");
                return filename;
            }
            catch (Exception e)
            {
                _logger.LogError($"downloading photo file failed: id {photo.ID}, {filename}, {e}");
            }
        }

        return string.Empty;
    }

    private async Task<(StatusProcess status, Message messageData)> WorkMessage(long channelId, MessageBase message)
    {
        if (message is not TL.Message m)
        {
            _logger.LogWarning($"message telegram has not message, type {message?.GetType()}");
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
    private async Task ProcessComments(Message message, InputPeerChannel peerChanel, int telegramMessageId)
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
