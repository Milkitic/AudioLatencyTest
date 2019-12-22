namespace AudioLatencyTest.Models
{
    public sealed class WaveOutInfo : DeviceInfo
    {
        public WaveOutInfo(int deviceNumber, string friendlyName)
        {
            DeviceNumber = deviceNumber;
            FriendlyName = friendlyName;
        }

        public override OutputMethod OutputMethod => OutputMethod.WaveOut;
        public int DeviceNumber { get; private set; }
    }
}