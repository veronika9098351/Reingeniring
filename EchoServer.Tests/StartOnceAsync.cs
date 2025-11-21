using NUnit.Framework;
using EchoServer;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EchoServer.Tests
{
    public class EchoServerTests
    {
        [Test]
        public async Task StartOnceAsync_ShouldEchoMessage()
        {
            int port = 6000;

            var server = new EchoServer(port);

            var serverTask = Task.Run(() => server.StartOnceAsync());

            var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", port);

            var stream = client.GetStream();
            byte[] message = { 5, 5, 5 };
            await stream.WriteAsync(message, 0, message.Length);

            byte[] buffer = new byte[100];
            int read = await stream.ReadAsync(buffer, 0, buffer.Length);

            Assert.AreEqual(message.Length, read);
            Assert.AreEqual(message, buffer[..read]);
        }
    }
}

