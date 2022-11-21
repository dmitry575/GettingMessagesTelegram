using GettingMessagesTelegram.Data;

namespace GettingMessagesTelegram.Services;

public interface IMessageService
{
    /// <summary>
    /// Get message by baseId
    /// </summary>
    /// <param name="channelId">Id channel</param>
    /// <param name="baseId">Id message to telegram</param>
    /// <param name="cancellationToken"></param>
    Task<Message> GetByBaseId(long channelId, long baseId, CancellationToken cancellationToken);

    /// <summary>
    /// Replace data
    /// </summary>
    /// <param name="message">Information for message</param>
    /// <param name="cancellationToken"></param>
    Task<int> ReplaceAsync(Message message, CancellationToken cancellationToken);

    /// <summary>
    /// Get message by group id
    /// </summary>
    /// <param name="channelId">Id channel</param>
    /// <param name="groupId">Group Id of message to telegram</param>
    /// <param name="cancellationToken"></param>
    Task<Message> GetByGroupId(long channelId, long groupId, CancellationToken cancellationToken);

    /// <summary>
    /// Get message by message info into the transaction
    /// </summary>
    /// <param name="message">message from telegram</param>
    /// <param name="cancellationToken"></param>
    Task<(Message, bool)> GetCreateByMessage(Message message, CancellationToken cancellationToken);

    /// <summary>
    /// Get message by Id
    /// </summary>
    /// <param name="id">Id message </param>
    /// <param name="withTranslates">With translates messages</param>
    Task<Message> GetById(long id, bool withTranslates = false);

    /// <summary>
    /// Get not translate messages
    /// </summary>
    /// <param name="language">Language for searching not translating messages</param>
    /// <param name="page">Current page</param>
    /// <param name="countRows">Count of row on witch page</param>
    Task<List<Message>> GetNotTranslate(string language, int page, int countRows);

    /// <summary>
    /// Get not translate message with empty content
    /// </summary>
    /// <param name="language">Language for searching not translating messages</param>
    /// <param name="lastId">Id message from get rows</param>
    /// <param name="countRows">Count of row on witch page</param>
    Task<List<Message>> GetNotTranslateEmptyContent(string language, long lastId, int countRows);
}
