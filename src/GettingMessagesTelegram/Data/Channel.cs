namespace GettingMessagesTelegram.Data;

/// <summary>
/// Description of channels wich your account subscribe
/// </summary>
public sealed class Channel
{
    /// <summary>
    /// Id channel
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Id channel into Telegram
    /// </summary>
    public long BaseId { get; set; }
    
    /// <summary>
    /// Author of channel
    /// </summary>
    public string Author { get; set; }
    
    /// <summary>
    /// Hash of access
    /// </summary>
    public long HashAccess { get; set; }
    
    /// <summary>
    /// Date create
    /// </summary>
    public DateTime DateCreated { get; set; }
    
    /// <summary>
    /// Count of messages
    /// </summary>
    public long MessagesCount { get; set; }
    
    public IEnumerable<Message> Messages { get; set; }
}
