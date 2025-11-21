using EchoServer;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServer
{
    public class EchoServer
    {
        private readonly int _port;
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly IClientHandler _handler;  // <-- додано

        public bool IsRunning { get; private set; }

        public EchoServer(int port,
                          IClientHandler? handler = null,
                          TcpListener? listener = null)
        {
            _port = port;
            _listener = listener ?? new TcpListener(IPAddress.Any, port);
            _cancellationTokenSource = new CancellationTokenSource();

            // якщо хендлер не передали — використовуємо наш
            _handler = handler ?? new EchoClientHandler(new EchoProcessor());
        }

        public async Task StartAsync()
        {
            IsRunning = true;
            _listener.Start();
            Console.WriteLine($"Server started on port {_port}.");

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client, _cancellationTokenSource.Token));
            }

            Console.WriteLine("Server shutdown.");
        }

        // однократний режим для тестів
        public async Task StartOnceAsync()
        {
            IsRunning = true;
            _listener.Start();

            TcpClient client = await _listener.AcceptTcpClientAsync();
            await HandleClientAsync(client, _cancellationTokenSource.Token);
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            // тепер логіку обробки клієнта виконує Handler
            await _handler.HandleAsync(client, token);
        }

        public void Stop()
        {
            IsRunning = false;
            _cancellationTokenSource.Cancel();
            _listener.Stop();
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            EchoServer server = new EchoServer(5000);
            await server.StartAsync();
        }
    }
}
