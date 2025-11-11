
public interface IUdpClient
{
    event EventHandler<byte[]>? MessageReceived;

    Task StartListeningAsync();

    void StopListening();
    void Exit();
    int GetHashCode();
}