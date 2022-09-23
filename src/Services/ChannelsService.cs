using TL;
using WTelegram;

namespace GettingMessagesTelegram.Services;

public class ChannelsService
{
    private readonly Client _clientTelegram;

    public ChannelsService(Client clientTelegram)
    {
        _clientTelegram = clientTelegram;
    }

    public async Task Work()
    {
        var my = await _clientTelegram.LoginUserIfNeeded();
        try
        {
            var mes = await _clientTelegram.Messages_GetHistory(new InputPeerChannel(1101806611, 6504238671879902293));
            foreach (var mesMessage in mes.Messages)
            {
                if (mesMessage is Message m)
                {
                    Console.WriteLine(m.ID + "\r\n" + m.message + "\r\n" + m.post_author);
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
}
