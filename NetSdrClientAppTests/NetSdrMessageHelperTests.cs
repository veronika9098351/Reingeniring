using NetSdrClientApp.Messages;

namespace NetSdrClientAppTests
{
    public class NetSdrMessageHelperTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetControlItemMessageTest()
        {
            //Arrange
            var type = NetSdrMessageHelper.MsgTypes.Ack;
            var code = NetSdrMessageHelper.ControlItemCodes.ReceiverState;
            int parametersLength = 7500;

            //Act
            byte[] msg = NetSdrMessageHelper.GetControlItemMessage(type, code, new byte[parametersLength]);

            var headerBytes = msg.Take(2);
            var codeBytes = msg.Skip(2).Take(2);
            var parametersBytes = msg.Skip(4);

            var num = BitConverter.ToUInt16(headerBytes.ToArray());
            var actualType = (NetSdrMessageHelper.MsgTypes)(num >> 13);
            var actualLength = num - ((int)actualType << 13);
            var actualCode = BitConverter.ToInt16(codeBytes.ToArray());

            //Assert
            Assert.That(headerBytes.Count(), Is.EqualTo(2));
            Assert.That(msg.Length, Is.EqualTo(actualLength));
            Assert.That(type, Is.EqualTo(actualType));

            Assert.That(actualCode, Is.EqualTo((short)code));

            Assert.That(parametersBytes.Count(), Is.EqualTo(parametersLength));
        }

        [Test]
        public void GetDataItemMessageTest()
        {
            //Arrange
            var type = NetSdrMessageHelper.MsgTypes.DataItem2;
            int parametersLength = 7500;

            //Act
            byte[] msg = NetSdrMessageHelper.GetDataItemMessage(type, new byte[parametersLength]);

            var headerBytes = msg.Take(2);
            var parametersBytes = msg.Skip(2);

            var num = BitConverter.ToUInt16(headerBytes.ToArray());
            var actualType = (NetSdrMessageHelper.MsgTypes)(num >> 13);
            var actualLength = num - ((int)actualType << 13);

            //Assert
            Assert.That(headerBytes.Count(), Is.EqualTo(2));
            Assert.That(msg.Length, Is.EqualTo(actualLength));
            Assert.That(type, Is.EqualTo(actualType));

            Assert.That(parametersBytes.Count(), Is.EqualTo(parametersLength));
        }

        //TODO: add more NetSdrMessageHelper tests
    }
}
[Test]
public void GetControlItemMessage_ReturnsNotNull()
{
    var msg = NetSdrMessageHelper.GetControlItemMessage(
        NetSdrMessageHelper.MsgTypes.Ack,
        NetSdrMessageHelper.ControlItemCodes.ReceiverState,
        new byte[10]
    );
    Assert.That(msg, Is.Not.Null);
}

[Test]
public void GetDataItemMessage_ReturnsCorrectLength()
{
    var msg = NetSdrMessageHelper.GetDataItemMessage(
        NetSdrMessageHelper.MsgTypes.DataItem2,
        new byte[100]
    );
    Assert.That(msg.Length, Is.GreaterThan(0));
}

[Test]
public void ControlItemMessage_HasExpectedHeader()
{
    var msg = NetSdrMessageHelper.GetControlItemMessage(
        NetSdrMessageHelper.MsgTypes.Ack,
        NetSdrMessageHelper.ControlItemCodes.ReceiverState,
        new byte[50]
    );
    var header = BitConverter.ToUInt16(msg, 0);
    Assert.That(header, Is.GreaterThan(0));
}
