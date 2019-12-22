using System;

namespace AudioLatencyTest.Models
{
    public sealed class DirectSoundOutInfo : DeviceInfo
    {
        public DirectSoundOutInfo(Guid deviceGuid, string friendlyName)
        {
            DeviceGuid = deviceGuid;
            FriendlyName = friendlyName;
        }

        public override OutputMethod OutputMethod => OutputMethod.DirectSound;
        public Guid DeviceGuid { get; private set; }
    }
}