using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// This program was designed for test purposes only
/// Not for a review
/// </summary>
public class EchoServer
{
    private readonly int _port;
    private TcpListener _listener;
    private CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning { get; private set; }

        public EchoServer(int port,
                          TcpListener? listener = null)
        {
            _port = port;
            _listener = listener ?? new TcpListener(IPAddress.Any, port);
            _cancellationTokenSource = new CancellationTokenSource();
        }
        }
        public void Stop()
        {
            IsRunning = false;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose(); // <-- SonarCloud FIX
            _listener.Stop();
        }
    }


public class UdpTimedSender : IDisposable
{
    private readonly string _host;
    private readonly int _port;
    private readonly UdpClient _udpClient;
    private Timer _timer;

    public UdpTimedSender(string host, int port)
    {
        _host = host;
        _port = port;
        _udpClient = new UdpClient();
    }

    public void StartSending(int intervalMilliseconds)
    {
        if (_timer != null)
            throw new InvalidOperationException("Sender is already running.");

        _timer = new Timer(SendMessageCallback, null, 0, intervalMilliseconds);
    }

    ushort i = 0;

    private void SendMessageCallback(object state)
    {
        try
        {
            //dummy data
            Random rnd = new Random();
            byte[] samples = new byte[1024];
            rnd.NextBytes(samples);
            i++;

            byte[] msg = (new byte[] { 0x04, 0x84 }).Concat(BitConverter.GetBytes(i)).Concat(samples).ToArray();
            var endpoint = new IPEndPoint(IPAddress.Parse(_host), _port);

            _udpClient.Send(msg, msg.Length, endpoint);
            Console.WriteLine($"Message sent to {_host}:{_port} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public void StopSending()
    {
        _timer?.Dispose();
        _timer = null;
    }

    public void Dispose()
    {
        StopSending();
        _udpClient.Dispose();
    }
}