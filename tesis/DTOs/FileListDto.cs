// tesis/DTOs/FileListDto.cs
namespace tesis.DTOs
{
    public class FileListDto
    {
        public string deviceId { get; set; } = string.Empty;
        public List<FileItemDto> files { get; set; } = new List<FileItemDto>();
    }

    public class FileItemDto
    {
        public string name { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string size { get; set; } = string.Empty;
    }
}