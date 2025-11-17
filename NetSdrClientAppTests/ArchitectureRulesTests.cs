using NUnit.Framework;
using NetArchTest.Rules;

namespace NetSdrClientAppTests
{
    [TestFixture]
    public class ArchitectureRulesTests
    {
        private const string AppNamespace = "NetSdrClientApp";

        private static Types AppTypes =>
            Types
                .InAssembly(typeof(NetSdrClientApp.NetSdrClient).Assembly); // <— тільки головна збірка

        [Test]
        public void ClientApp_ShouldNotDependOn_EchoServer()
        {
            var result = AppTypes
                .ShouldNot()
                .HaveDependencyOn("EchoServer")
                .GetResult();

            Assert.That(result.IsSuccessful, Is.True);
        }

        [Test]
        public void ClientApp_ShouldNotDependOn_TestProject()
        {
            var result = AppTypes
                .ShouldNot()
                .HaveDependencyOn("NetSdrClientAppTests")
                .GetResult();

            Assert.That(result.IsSuccessful);
        }
    }
}
