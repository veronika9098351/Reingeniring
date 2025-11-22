using EchoServer;
using EchoServer.Interfaces;
using FluentAssertions;
using Moq;

namespace EchoServerTests
{
    [TestFixture]
    public class EchoTcpServerTests
    {
        private Mock<ILogger> _loggerMock;
        private MessageHandler _messageHandler;
        private EchoTcpServer _server;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _messageHandler = new MessageHandler(_loggerMock.Object);
            _server = new EchoTcpServer(5000, _loggerMock.Object, _messageHandler);
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsException()
        {
            var handler = new MessageHandler(new Mock<ILogger>().Object);

            Action act = () => new EchoTcpServer(5000, null, handler);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("logger");
        }

        [Test]
        public void Constructor_WithNullMessageHandler_ThrowsException()
        {
            Action act = () => new EchoTcpServer(5000, _loggerMock.Object, null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("messageHandler");
        }

        [Test]
        public void Stop_DoesNotThrow()
        {
            Action act = () => _server.Stop();

            act.Should().NotThrow();
            _loggerMock.Verify(l => l.Log("Server stopped."), Times.Once);
        }
    }
}