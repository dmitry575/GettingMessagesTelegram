﻿using TL;

namespace GettingMessagesTelegram.Extensions;

public static class DataMessage
{
    /// <summary>
    /// Mapping data
    /// </summary>
    /// <param name="message">Message from telegram</param>
    public static Data.Message Map(this Message message)
    {
        return new Data.Message
        {
            BaseId = message.id,
            Author = message.post_author,
            Content = message.message,
            DateCreated = message.Date,
            ViewCount = message.views
        };
    }
}
