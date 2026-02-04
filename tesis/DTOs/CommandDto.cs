namespace tesis.DTOs
{
    public class CommandDto
    {
        public string DeviceId { get; set; } = string.Empty; // ¿A quién atacamos?
        public string Command { get; set; } = string.Empty;  // ¿Qué le hacemos? (TAKE_PHOTO)
    }
}