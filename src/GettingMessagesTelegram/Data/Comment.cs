﻿namespace GettingMessagesTelegram.Data;

public class Comment
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
    /// Author of message id
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Content of message
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// DateCreated message
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Id of relation message
    /// </summary>
    public long MessageId { get; set; }

    /// <summary>
    /// Date published comment
    /// </summary>
    public DateTime? PublishData { get; set; }

    public virtual Message Message { get; set; }

    public virtual ICollection<CommentTranslate> Translates { get; set; }
}
