using Microsoft.Extensions.Hosting;

namespace TranslateService.Services
{
    public class TranslateService : BackgroundService
    {
        private readonly ITranslateMessages _translateMessages;

        public TranslateService(ITranslateMessages translateMessages)
        {
            _translateMessages = translateMessages;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _translateMessages.Translate(stoppingToken);
        }
    }
}
