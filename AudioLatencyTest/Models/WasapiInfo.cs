using NAudio.CoreAudioApi;

namespace AudioLatencyTest.Models
{
    public sealed class WasapiInfo : DeviceInfo
    {
        public WasapiInfo(MMDevice device, string friendlyName)
        {
            Device = device;
            FriendlyName = friendlyName;
        }

        public override OutputMethod OutputMethod => OutputMethod.Wasapi;
        public MMDevice Device { get; private set; }
    }
}