namespace AudioLatencyTest.Models
{
    public abstract class DeviceInfo
    {
        public abstract OutputMethod OutputMethod { get; }
        public string FriendlyName { get; protected set; }
        public override string ToString()
        {
            return $"({OutputMethod}) {FriendlyName}";
        }
    }
}
