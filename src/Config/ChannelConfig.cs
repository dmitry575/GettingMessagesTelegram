namespace GettingMessagesTelegram.Config;

/// <summary>
/// Configuration of one channel
/// </summary>
public class ChannelConfig
{
    /// <summary>
    /// Id channel in telegram
    /// If you do not know Id of channel, you should use the method of API:
    /// _clientTelegram.Messages_GetAllChats() 
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Hash for access to channel
    /// </summary>
    public long HashAccess { get; set; }
    
    /// <summary>
    /// User name
    /// </summary>
    public string Name { get; set; }
    
}
