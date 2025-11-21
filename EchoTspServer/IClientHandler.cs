using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServer
{
    public interface IClientHandler
    {
        Task HandleAsync(TcpClient client, CancellationToken token);
    }
}
