namespace GettingMessagesTelegram.Services;

public interface IChannelsService
{
  Task WorkAsync(CancellationToken cancellationToken);
}
