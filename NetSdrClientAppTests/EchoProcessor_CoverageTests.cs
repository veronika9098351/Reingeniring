using NUnit.Framework;

namespace EchoServer.Tests
{
    public class EchoProcessor_CoverageTests
    {
        [Test]
        public void Process_ShouldReturnSameBytes()
        {
            var processor = new EchoProcessor();
            byte[] input = { 10, 20, 30 };

            var result = processor.Process(input, input.Length);

            Assert.That(result, Is.EqualTo(input));
        }
    }
}
