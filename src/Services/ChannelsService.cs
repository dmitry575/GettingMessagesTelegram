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
       var chanels = await _clientTelegram.Channels_GetChannels(new[] { new InputChannel(-1001101806611, 0) });
       Console.WriteLine(chanels.chats.Count);
    }
}
