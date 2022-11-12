using TL;

namespace GettingMessagesTelegram.Extensions;

public static class DataMessage
{
    /// <summary>
    /// Mapping data
    /// </summary>
    /// <param name="message">Message from telegram</param>
    /// <param name="defaultAuthor">Default name of message. Message without author must not exists</param>
    /// <param name="channelId">Channel id</param>
    public static Data.Message Map(this Message message, string defaultAuthor, long channelId)
    {
        return new Data.Message
        {
            ChannelId = channelId,
            BaseId = message.id,
            GroupId = message.grouped_id,
            Author = string.IsNullOrEmpty(message.post_author) ? defaultAuthor : message.post_author,
            Content = message.message,
            DateCreated = message.Date,
            ViewCount = message.views
        };
    }

    /// <summary>
    /// Mapping to comment
    /// </summary>
    /// <param name="message">Message from telegram</param>
    public static Data.Comment MapToComment(this Message message)
    {
        return new Data.Comment
        {
            BaseId = message.id,
            Author = message.post_author ?? "user " + message.from_id?.ID,
            UserId = message.from_id?.ID,
            Content = message.message,
            DateCreated = message.Date
        };
    }
}
