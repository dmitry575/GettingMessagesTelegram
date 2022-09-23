// See https://aka.ms/new-console-template for more information

using GettingMessagesTelegram.Services;

static string Config(string what)
{
    switch (what)
    {
        case "api_id": return "71522";
        case "api_hash": return "****";
        case "phone_number": return "****";
        case "verification_code": Console.Write("Code: "); return Console.ReadLine();
        default: return null;
    }
}

using var client = new WTelegram.Client(Config);
var channelService = new ChannelsService(client);
await channelService.Work();
