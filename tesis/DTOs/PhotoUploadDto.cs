using Microsoft.AspNetCore.Http;

namespace tesis.DTOs
{
    public class PhotoUploadDto
    {
        // El nombre debe coincidir con lo que envía Flutter
        public string DeviceId { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
    }
}