using System;
using System.Linq;

namespace EchoServer
{
    public class MessageHandler
    {
        private readonly Interfaces.ILogger _logger;

        public MessageHandler(Interfaces.ILogger logger)
        {
            _logger = logger;
        }
        public byte[] Handle(byte[] message)
        {
            _logger.Log($"Received {message.Length} bytes");
            return message;
        }
    }
}
