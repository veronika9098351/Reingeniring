using NUnit.Framework;
using EchoServer;

namespace EchoServer.Tests
{
    public class EchoProcessorTests
    {
        [Test]
        public void Process_ShouldReturnSameBytes()
        {
            // FIXED for SonarCloud
            var processor = new EchoProcessor();

            byte[] input = { 1, 2, 3 };

            byte[] result = processor.Process(input, input.Length);

            // FIXED for SonarCloud (constraint model)
            Assert.That(result, Is.EqualTo(input));
        }
    }
}
