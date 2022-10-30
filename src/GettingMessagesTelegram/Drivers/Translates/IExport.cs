namespace GettingMessagesTelegram.Drivers.Translates
{
    /// <summary>
    /// Export messages for translating
    /// </summary>
    public interface IExport
    {
        Task ExportAsync(CancellationToken cancellation = default);
    }
}
