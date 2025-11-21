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
        private readonly CancellationTokenSource _cts;

        public bool IsRunning { get; private set; }

        public EchoServer(int port)
        {
            if (port <= 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));

            _port = port;
            _listener = new TcpListener(IPAddress.Any, port);
            _cts = new CancellationTokenSource();
        }

        public async Task StartOnceAsync()
        {
            _listener.Start();
            IsRunning = true;

            TcpClient client = await _listener.AcceptTcpClientAsync();
            await HandleClientAsync(client, _cts.Token);
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using var stream = client.GetStream();
            byte[] buffer = new byte[8192];

            int read = await stream.ReadAsync(buffer, token);
            if (read > 0)
            {
                await stream.WriteAsync(buffer.AsMemory(0, read), token);
            }

            client.Close();
        }

        public void Stop()
        {
            _cts.Cancel();
            _listener.Stop();
            IsRunning = false;
        }
    }
}

