using System.Text;
using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.Drivers.Translates.Config;
using GettingMessagesTelegram.Services;
using Microsoft.Extensions.Options;

namespace GettingMessagesTelegram.Drivers.Translates.Imp;

public class Export : IExport
{
    private readonly IMessageService _messageService;
    private readonly TranslatesConfig _config;

    public Export(IOptions<TranslatesConfig> config, IMessageService messageService)
    {
        _messageService = messageService;
        _config = config.Value;
    }

    public async Task ExportAsync(CancellationToken cancellation = default)
    {
    }
}
