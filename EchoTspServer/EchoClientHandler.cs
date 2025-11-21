using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServer
{
    public class EchoClientHandler : IClientHandler
    {
        private readonly IEchoProcessor _processor;

        public EchoClientHandler(IEchoProcessor processor)
        {
            _processor = processor;
        }

        public async Task HandleAsync(TcpClient client, CancellationToken token)
        {
            using var stream = client.GetStream();
            byte[] buffer = new byte[8192];

            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);

            var response = _processor.Process(buffer, bytesRead);

            await stream.WriteAsync(response, 0, response.Length, token);
        }
    }
}
