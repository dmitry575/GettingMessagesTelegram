using Microsoft.Extensions.Hosting;

namespace TranslateService.Services
{
    public class TranslateService : BackgroundService
    {
        public TranslateService()
        {
            

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
