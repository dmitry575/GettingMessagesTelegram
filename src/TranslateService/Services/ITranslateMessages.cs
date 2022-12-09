namespace TranslateService.Services
{
    public interface ITranslateMessages
    {
        Task Translate(CancellationToken stoppingToken);
    }
}
