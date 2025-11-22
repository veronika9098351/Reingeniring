namespace EchoServer
{
    public class MessageHandler
    {
        private readonly Interfaces.ILogger _logger;

        public MessageHandler(Interfaces.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public byte[] ProcessMessage(byte[] message)
        {
            if (message == null || message.Length == 0)
            {
                return Array.Empty<byte>();
            }

            _logger.Log($"Processing message of {message.Length} bytes");
            return message;
        }
    }
}