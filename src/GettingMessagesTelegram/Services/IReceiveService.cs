namespace GettingMessagesTelegram.Services;

public interface IReceiveService: IDisposable
{
    /// <summary>
    /// Main work, get history message and added their to the database
    /// </summary>
  Task WorkAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Turn on listener to new messages
    /// </summary>
    void SubscribeToEvents();
}
