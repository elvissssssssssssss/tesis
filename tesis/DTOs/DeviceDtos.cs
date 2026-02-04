namespace tesis.DTOs
{
    public class RegisterDeviceDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string OsVersion { get; set; } = string.Empty;
    }

    public class HeartbeatDto
    {
        public string DeviceId { get; set; } = string.Empty;
    }
}