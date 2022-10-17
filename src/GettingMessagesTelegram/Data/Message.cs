using GettingMessagesTelegram.DataAccess;

namespace GettingMessagesTelegram.Data;

public class Message
{
    /// <summary>
    /// Id message
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Id message into Telegram
    /// </summary>
    public long BaseId { get; set; }
    
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
    /// How many people viewed message
    /// </summary>
    public long ViewCount { get; set; }
    
    /// <summary>
    /// How many comments has message
    /// </summary>
    public int CommentCount { get; set; }
    
    /// <summary>
    /// Id of relation channel
    /// </summary>
    public long ChannelId { get; set; }
    
    public virtual Channel Channel { get; set; }
    
    public virtual ICollection<Comment> Comments { get; set; }
    
    public virtual ICollection<Media> Medias { get; set; }
}
