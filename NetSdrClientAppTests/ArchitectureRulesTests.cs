using NetArchTest.Rules;
using NUnit.Framework;

namespace NetSdrClientAppTests
{
    [TestFixture]
    public class ArchitectureRulesTests
    {
        private const string AppNamespace = "NetSdrClientApp";
        private const string ServerNamespace = "EchoServer";

        [Test]
        public void ClientApp_ShouldNotDependOn_EchoServer()
        {
            var result = Types
                .InCurrentDomain()
                .ShouldNot()
                .HaveDependencyOn(ServerNamespace)
                .GetResult();

            Assert.That(result.IsSuccessful, Is.True);
        }

        [Test]
        public void App_ShouldNotDependOn_TestProject()
        {
            var result = Types
                .InCurrentDomain()
                .ShouldNot()
                .HaveDependencyOn("NetSdrClientAppTests")
                .GetResult();

            Assert.That(result.IsSuccessful, Is.True);
        }
    }
}
