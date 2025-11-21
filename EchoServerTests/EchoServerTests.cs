using EchoServerApp;
using NUnit.Framework;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoServerTests
{
    public class EchoServerTests
    {
        [Test]
        public void EchoServer_CanBeCreated()
        {
            var server = new EchoServer(5000);
            Assert.That(server, Is.Not.Null);
        }

        [Test]
        public async Task EchoServer_HasListenerAfterStart()
        {
            var server = new EchoServer(5000);

            var task = server.StartOnceAsync();

            await Task.Delay(100); // даємо серверу стартувати

            Assert.That(server.IsRunning, Is.True);
        }
        [Test]
        public async Task EchoServer_AcceptsClient()
        {
            var server = new EchoServer(5001);
            var serverTask = server.StartOnceAsync();

            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 5001);

            Assert.That(client.Connected, Is.True);

            server.Stop();
        }
        [Test]
        public async Task EchoServer_EchoesBackMessage()
        {
            var server = new EchoServer(5002);
            var serverTask = server.StartOnceAsync();

            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 5002);

            var stream = client.GetStream();
            byte[] msg = Encoding.UTF8.GetBytes("hello");
            await stream.WriteAsync(msg, 0, msg.Length);

            byte[] buffer = new byte[5];
            int read = await stream.ReadAsync(buffer, 0, buffer.Length);

            string received = Encoding.UTF8.GetString(buffer);

            Assert.That(received, Is.EqualTo("hello"));

            server.Stop();
        }
        [Test]
        public void EchoServer_StopsCorrectly()
        {
            var server = new EchoServer(5003);

            Assert.That(server.IsRunning, Is.False);

            var startTask = server.StartOnceAsync();

            Assert.That(server.IsRunning, Is.True);

            server.Stop();
            Assert.That(server.IsRunning, Is.False);
        }

    }
}
