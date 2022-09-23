namespace GettingMessagesTelegram.Data;

public class Message
{
    /// <summary>
    /// Id channel
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Author of message
    /// </summary>
    public string Author { get; set; }
    
    /// <summary>
    /// Content of message
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// DateCreated message
    /// </summary>
    public DateTime DateCreated { get; set; }
    
    /// <summary>
    /// How many peoople viewed message
    /// </summary>
    public long ViewCount { get; set; }
    
    /// <summary>
    /// Id of relation channel
    /// </summary>
    public long ChannelId { get; set; }
    
    public virtual Channel Channel { get; set; }
}
