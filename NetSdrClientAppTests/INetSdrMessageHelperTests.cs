namespace NetSdrClientAppTests
{
    public interface INetSdrMessageHelperTests
    {
        void ControlItemMessage_HasExpectedHeader();
        void GetControlItemMessageReturnsNotNull();
        void GetControlItemMessageTest();
        void GetDataItemMessageTest();
        void GetDataItemMessage_ReturnsCorrectLength();
        void Setup();
    }
}