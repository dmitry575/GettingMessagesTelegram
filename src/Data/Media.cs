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
    
    /// <summary>
    /// Type of file
    /// </summary>
    public string MimeType { get; set; }
    
    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; }
    
    /// <summary>
    /// File size
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// Url on external web service for viewing users
    /// </summary>
    public string UrlExternal { get; set; }
    
    /// <summary>
    /// Local path where downloaded file
    /// </summary>
    public string LocalPath { get; set; }

    public virtual Message Message { get; set; }
}
