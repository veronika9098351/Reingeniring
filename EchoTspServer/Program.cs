using System.Net;
using System.Net.Sockets;

namespace EchoServer
{
    public class EchoTcpServer
    {
        private readonly int _port;
        private readonly Interfaces.ILogger _logger;
        private readonly MessageHandler _messageHandler;
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;

        public EchoTcpServer(int port, Interfaces.ILogger logger, MessageHandler messageHandler)
        {
            _port = port;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _logger.Log($"Server started on port {_port}.");

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _logger.Log("Client connected.");
                    _ = Task.Run(() => HandleClientAsync(client, _cancellationTokenSource.Token));
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }

            _logger.Log("Server shutdown.");
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using (NetworkStream stream = client.GetStream())
            {
                try
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;

                    while (!token.IsCancellationRequested && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                    {
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(buffer, receivedData, bytesRead);
                        byte[] response = _messageHandler.ProcessMessage(receivedData);
                        await stream.WriteAsync(response, 0, response.Length, token);
                        _logger.Log($"Echoed {response.Length} bytes to the client.");
                    }
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    _logger.Log($"Error: {ex.Message}");
                }
                finally
                {
                    client.Close();
                    _logger.Log("Client disconnected.");
                }
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _listener?.Stop();
            _cancellationTokenSource.Dispose();
            _logger.Log("Server stopped.");
        }
    }

    public class UdpTimedSender : IDisposable
    {
        private readonly string _host;
        private readonly int _port;
        private readonly UdpClient _udpClient;
        private Timer _timer;
        private ushort _counter = 0;

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

        private void SendMessageCallback(object state)
        {
            try
            {
                Random rnd = new Random();
                byte[] samples = new byte[1024];
                rnd.NextBytes(samples);
                _counter++;
                byte[] msg = (new byte[] { 0x04, 0x84 }).Concat(BitConverter.GetBytes(_counter)).Concat(samples).ToArray();
                var endpoint = new IPEndPoint(IPAddress.Parse(_host), _port);
                _udpClient.Send(msg, msg.Length, endpoint);
                Console.WriteLine($"Message sent to {_host}:{_port}");
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

    class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = new ConsoleLogger();
            var messageHandler = new MessageHandler(logger);
            var server = new EchoTcpServer(5000, logger, messageHandler);

            _ = Task.Run(() => server.StartAsync());

            string host = "127.0.0.1";
            int port = 60000;
            int intervalMilliseconds = 5000;

            using (var sender = new UdpTimedSender(host, port))
            {
                Console.WriteLine("Press any key to stop sending...");
                sender.StartSending(intervalMilliseconds);
                Console.WriteLine("Press 'q' to quit...");
                while (Console.ReadKey(intercept: true).Key != ConsoleKey.Q) { }
                sender.StopSending();
                server.Stop();
                Console.WriteLine("Sender stopped.");
            }
        }
    }
}