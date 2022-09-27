﻿using GettingMessagesTelegram.Data;

namespace GettingMessagesTelegram.Services;

public interface IMessageService
{
    /// <summary>
    /// Get message by baseId
    /// </summary>
    /// <param name="channelId">Id channel</param>
    /// <param name="baseId">Id message to telegram</param>
    Task<Message> GetByBaseId(long channelId, long baseId);
}
