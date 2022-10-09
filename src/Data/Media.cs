using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Enums;

namespace GettingMessagesTelegram.DataAccess;

public class Media
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
    /// Id of relation message
    /// </summary>
    public long MessageId { get; set; }
    
    /// <summary>
    /// Hash of access
    /// </summary>
    public long HashAccess { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description{ get; set; }

    /// <summary>
    /// Url
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Hash of access
    /// </summary>
    public MediaType Type { get; set; }
    
    public virtual Message Message { get; set; }
    public MediaType MediaType { get; internal set; }
}
