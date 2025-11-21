using NUnit.Framework;
using EchoServer;

namespace EchoServer.Tests
{
    public class EchoProcessorTests
    {
        [Test]
        public void Process_ShouldReturnSameBytes()
        {
            IEchoProcessor processor = new EchoProcessor();
            byte[] input = { 1, 2, 3 };

            byte[] result = processor.Process(input, input.Length);

            Assert.AreEqual(input, result);
        }
    }
}
