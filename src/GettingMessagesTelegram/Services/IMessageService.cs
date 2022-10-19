using GettingMessagesTelegram.Data;

namespace GettingMessagesTelegram.Services;

public interface IMessageService
{
    /// <summary>
    /// Get message by baseId
    /// </summary>
    /// <param name="channelId">Id channel</param>
    /// <param name="baseId">Id message to telegram</param>
    Task<Message> GetByBaseId(long channelId, long baseId);

    /// <summary>
    /// Replace data
    /// </summary>
    /// <param name="message">Information fo messsage</param>
    /// <param name="cancellationToken"></param>
    Task<int> ReplaceAsync(Message message, CancellationToken cancellationToken);

    /// <summary>
    /// Get message by group id
    /// </summary>
    /// <param name="channelId">Id channel</param>
    /// <param name="groupId">Group Id of message to telegram</param>
    Task<Message> GetByGroupId(long channelId, long groupId);
}
