namespace GettingMessagesTelegram.Services;

public interface IReceiveService
{
  Task WorkAsync(CancellationToken cancellationToken);
}
