using NUnit.Framework;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EchoServer.Tests
{
    public class EchoServer_CoverageTests
    {
        [Test]
        public async Task StartOnceAsync_ShouldAcceptClient()
        {
            var server = new EchoServer(6000);

            var serverTask = Task.Run(() => server.StartOnceAsync());

            var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 6000);

            var stream = client.GetStream();
            byte[] msg = { 9, 9, 9 };

            await stream.WriteAsync(msg.AsMemory(0, msg.Length));

            byte[] buf = new byte[100];
            await stream.ReadAsync(buf.AsMemory(0, buf.Length));

            Assert.That(server.IsRunning, Is.True);
        }
    }
}
