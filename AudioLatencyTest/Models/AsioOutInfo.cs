namespace AudioLatencyTest.Models
{
    public sealed class AsioOutInfo : DeviceInfo
    {
        public AsioOutInfo(string friendlyName)
        {
            FriendlyName = friendlyName;
        }

        public override OutputMethod OutputMethod => OutputMethod.Asio;
    }
}