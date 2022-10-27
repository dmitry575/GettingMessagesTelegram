
namespace GettingMessagesTelegram.Process
{
    public interface  IBackgroundTaskQueue
    {
        /// <summary>
        /// Add some function for background execute
        /// </summary>
        ValueTask AddItemAsync(Func<CancellationToken, ValueTask> workItem);

        /// <summary>
        /// Get and delete from queue
        /// </summary>
        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    }
}
