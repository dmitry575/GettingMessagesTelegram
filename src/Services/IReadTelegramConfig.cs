namespace GettingMessagesTelegram.Services;

/// <summary>
/// Reading data for configuration of telegram client
/// If user not authorizate on this device, he must input special code from Telegram App
/// </summary>
public interface IReadTelegramConfig
{
    /// <summary>
    /// Read data depending of params
    /// api_id
    /// api_hash
    /// phone_number
    /// verification_code
    /// </summary>
    /// <param name="what">What param need for Telegram Client</param>
    string Read(string what);
}
