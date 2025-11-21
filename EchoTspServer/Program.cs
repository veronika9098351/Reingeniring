using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServerApp
{
    public class EchoServer
    {
        private readonly int _port;
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning { get; private set; }   // <-- додано

        public EchoServer(int port, TcpListener? listener = null)
        {
            _port = port;
            _listener = listener ?? new TcpListener(IPAddress.Any, port);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            IsRunning = true;                         // <-- додано
            _listener.Start();
            Console.WriteLine($"Server started on port {_port}.");

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client, _cancellationTokenSource.Token));
            }

            Console.WriteLine("Server shutdown.");
        }

        // для тестів
        public async Task StartOnceAsync()
        {
            IsRunning = true;                        // <-- додано
            _listener.Start();

            TcpClient client = await _listener.AcceptTcpClientAsync();
            await HandleClientAsync(client, _cancellationTokenSource.Token);
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[8192];
            int bytesRead;

            while (!token.IsCancellationRequested &&
                   (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
            {
                await stream.WriteAsync(buffer, 0, bytesRead, token);
            }

            client.Close();
        }

        public void Stop()
        {
            IsRunning = false;                       // <-- додано

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
