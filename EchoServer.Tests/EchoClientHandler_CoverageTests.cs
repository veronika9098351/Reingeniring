using NUnit.Framework;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EchoServer.Tests
{
    public class EchoClientHandler_CoverageTests
    {
        [Test]
        public async Task Handler_ShouldEchoBack()
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
            byte[] msg = { 1, 2, 3 };

            await stream.WriteAsync(msg.AsMemory(0, msg.Length));

            await handler.HandleAsync(accepted, default);

            byte[] buf = new byte[100];
            int read = await stream.ReadAsync(buf.AsMemory(0, buf.Length));

            Assert.That(buf[..3], Is.EqualTo(msg));
        }
    }
}
