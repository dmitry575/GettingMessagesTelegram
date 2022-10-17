using GettingMessagesTelegram.Data;

namespace GettingMessagesTelegram.Services;

/// <summary>
/// Work under the channels
/// </summary>
public interface IChannelsService
{
    /// <summary>
    /// Check exists and add if not exists
    /// </summary>
    /// <param name="baseId">Id channel</param>
    /// <param name="hashAccess">Hash of access</param>
    /// <param name="author">User name</param>
    /// <returns></returns>
    Task<Channel> CheckAdd(long baseId, long hashAccess, string author);
}
