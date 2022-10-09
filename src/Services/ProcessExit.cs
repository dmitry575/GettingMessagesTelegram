namespace GettingMessagesTelegram.Services;

public class Processes
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    public Processes(CancellationTokenSource cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;
    }

    public void ProcessExit(object sender, EventArgs e)
    {
        Console.WriteLine("exit from program");
        _cancellationTokenSource.Cancel();
    }
}
