namespace GettingMessagesTelegram.Services.Impl;

public class ReadTelegramConfig : IReadTelegramConfig
{
    TelegramConfig
    public string Read(string what)
    {
        switch (what)
        {
            case "api_id": return "71522";
            case "api_hash": return "9a871fe9b5c3abad4786d6ea693b0228";
            case "phone_number": return "+79172552240";
            case "verification_code":
                Console.Write("Code: ");
                return Console.ReadLine();
            default: return null;
        }
    }
}
