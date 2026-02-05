using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("device_files")] // Asegúrate de que este nombre coincida con tu SQL
public class DeviceFile
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("device_id")]
    public string DeviceId { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [Column("file_type")]
    public string FileType { get; set; } = string.Empty;

    [Column("file_size")]
    public string FileSize { get; set; } = string.Empty;
}