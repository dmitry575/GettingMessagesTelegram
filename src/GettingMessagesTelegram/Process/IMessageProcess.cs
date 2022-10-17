using GettingMessagesTelegram.Enums;
using TL;

namespace GettingMessagesTelegram.Process;

/// <summary>
/// Processing message
/// </summary>
public interface IMessageProcess
{
    /// <summary>
    /// Processing message
    /// Check exists in database and add if need
    /// </summary>
    /// <param name="channel">Channel information in our system</param>
    /// <param name="message">Message from telegram</param>
    /// <param name="cancellationToken">Token</param>
    Task<(StatusProcess, Data.Message)> Processing(Data.Channel channel, MessageBase message, CancellationToken cancellationToken = default);
}
