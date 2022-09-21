// See https://aka.ms/new-console-template for more information

using GettingMessagesTelegram.Services;

static string Config(string what)
{
    switch (what)
    {
        case "api_id": return "5781151608:AAEU-nS4RORZYaUZzbcaWOmGA7_60AirUII";
        case "api_hash": return "5781151608:AAEU-nS4RORZYaUZzbcaWOmGA7_60AirUII";
        default: return null;
    }
}

using var client = new WTelegram.Client(Config);
var channelService = new ChannelsService(client);
await channelService.Work();
