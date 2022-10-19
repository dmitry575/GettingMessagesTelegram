using GettingMessagesTelegram.Config;

namespace GettingMessagesTelegram.Services.Impl;

public class ReadTelegramConfig : IReadTelegramConfig
{
    private readonly TelegramConfig _telegramConfig;

    public ReadTelegramConfig(TelegramConfig telegramConfig)
    {
        _telegramConfig = telegramConfig;
    }

    public string Read(string what)
    {
        switch (what)
        {
            case "api_id": return _telegramConfig.ApiId;
            case "api_hash": return _telegramConfig.ApiHash;
            case "phone_number": return _telegramConfig.PhoneNumber;
            
            // if you first time to execute application you need to create session on this device
            case "verification_code":
                Console.Write("Code: ");
                return Console.ReadLine();
            default: return null;
        }
    }
}
