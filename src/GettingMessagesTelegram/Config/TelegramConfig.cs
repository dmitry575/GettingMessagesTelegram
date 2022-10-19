namespace GettingMessagesTelegram.Config;

/// <summary>
/// All fields of this configuration you should get on https://my.telegram.org/apps
/// </summary>
public class TelegramConfig
{
    /// <summary>
    /// Api id of application 
    /// </summary>
    public string ApiId { get; set; }
    
    /// <summary>
    /// Api hash of application 
    /// </summary>
    public string ApiHash { get; set; }
    
    /// <summary>
    /// User phone number, where user already added all channels 
    /// </summary>
    public string PhoneNumber { get; set; }
    
}
