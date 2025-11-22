namespace EchoServer.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
    }
}

namespace EchoServer
{
    public class ConsoleLogger : Interfaces.ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}