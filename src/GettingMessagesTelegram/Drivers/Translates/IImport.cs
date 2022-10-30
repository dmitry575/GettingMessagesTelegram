namespace GettingMessagesTelegram.Drivers.Translates
{
    /// <summary>
    /// Import messages and comments
    /// </summary>
    public interface IImport
    {
        Task ImportAsync(CancellationToken cancellation = default);
    }
}
