using GettingMessagesTelegram.Helpers;
using Microsoft.Extensions.Logging;
using TL;
using WTelegram;

namespace GettingMessagesTelegram.Services;

public class ChannelsService : IChannelsService
{
    private readonly Client _clientTelegram;
    private readonly ILogger<ChannelsService> _logger;

    public ChannelsService(Client clientTelegram, ILogger<ChannelsService> logger)
    {
        _clientTelegram = clientTelegram;
        _logger = logger;
    }

    public async Task WorkAsync(CancellationToken cancellationToken)
    {
        var me = await _clientTelegram.LoginUserIfNeeded();
        _logger.LogInformation($"Loggin by: {me.first_name}");
        try
        {
            var mes = await _clientTelegram.Messages_GetHistory(new InputPeerChannel(1101806611, 6504238671879902293));
            
            foreach (var mesMessage in mes.Messages)
            {
                if (mesMessage is Message m)
                {
                    if (MessageExists(m.grouped_id, m.ID))
                    {
                        _logger.LogInformation("found last exists message: "+m.ID + "\t" + m.message + "\t" + m.post_author);    
                        break;
                    }
                    _logger.LogInformation(m.ID + "\t" + m.message + "\t" + m.post_author);
//_clientTelegram.Channels_ExportMessageLink()
                    var link = UrlHelper.GetTmeUrl(m.message);
                    if (!string.IsNullOrEmpty(link))
                    {
                        var linkInfo = await _clientTelegram.Help_GetDeepLinkInfo(link);
                        _logger.LogInformation(linkInfo?.message);
                        var path = link.Replace("https://t.me/", "");
                        linkInfo = await _clientTelegram.Help_GetDeepLinkInfo(path);
                        _logger.LogInformation(linkInfo?.message);
                        var ll = await _clientTelegram.Help_GetRecentMeUrls(link);
                        //_logger.LogInformation(ll?.urls.Length);
                    }


                }
            }
            //var mess  =await _clientTelegram.GetMessages(new InputPeerChannel(1101806611, 6504238671879902293));

            var channels =
                await _clientTelegram.Channels_GetChannels(new[] { new InputChannel(1101806611, 6504238671879902293) });
            foreach (var channelsChat in channels.chats)
            {
                Console.WriteLine($"{channelsChat.Key}:  {channelsChat.Value.Title}");
                var messages =
                    await _clientTelegram.GetMessages(new InputPeerChannel(1001101806611, 6504238671879902293));
                foreach (var message in messages.Messages)
                {
                    Console.WriteLine($"message: {message.From.ID}, {message.Peer.ID}, {message.Date}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

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
        // Console.WriteLine(chanels.chats.Count);
    }

    /// <summary>
    /// If exists messages
    /// </summary>
    private bool MessageExists(long channelId, long messageId)
    {
        return false;
    }
}
