using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServer
{
    public class EchoTcpServer
    {
        private readonly int _port;
        private readonly Interfaces.ILogger _logger;
        private readonly MessageHandler _messageHandler;

        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning { get; private set; }

        public EchoTcpServer(int port, Interfaces.ILogger logger, MessageHandler messageHandler)
        {
            _port = port;
            _logger = logger;
            _messageHandler = messageHandler;

            _listener = new TcpListener(IPAddress.Any, _port);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            _listener.Start();

            _logger.Log($"TCP Server started on port: {_port}");

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }
        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using var stream = client.GetStream();
                byte[] buffer = new byte[4096];

                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    byte[] request = new byte[bytesRead];
                    Array.Copy(buffer, request, bytesRead);

                    byte[] response = _messageHandler.Handle(request);

                    await stream.WriteAsync(response, 0, response.Length);
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Client error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _listener.Stop();

            _logger.Log("TCP Server stopped.");
        }
    }
}
