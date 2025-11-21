using NUnit.Framework;
using EchoServer;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServer.Tests
{
    public class EchoClientHandlerTests
    {
        [Test]
        public async Task HandleAsync_ShouldEchoBackData()
        {
            var processor = new EchoProcessor();
            var handler = new EchoClientHandler(processor);

            var server = new TcpListener(IPAddress.Loopback, 0);
            server.Start();
            int port = ((IPEndPoint)server.LocalEndpoint).Port;

            var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", port);

            var accepted = await server.AcceptTcpClientAsync();

            var stream = client.GetStream();
            byte[] message = { 9, 9, 9 };

            await stream.WriteAsync(message.AsMemory(0, message.Length), CancellationToken.None); ;
            await handler.HandleAsync(accepted, CancellationToken.None);

            byte[] buffer = new byte[100];
            int read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(read, Is.EqualTo(3));
                Assert.That(buffer[..3], Is.EqualTo(message));
            }
            );
        }
        }
    }
