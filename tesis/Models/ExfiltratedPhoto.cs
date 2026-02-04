using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tesis.Models
{
    [Table("exfiltrated_photos")]
    public class ExfiltratedPhoto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("device_id")]
        [ForeignKey("Device")]
        public string DeviceId { get; set; } = string.Empty;
        public Device? Device { get; set; }

        [Column("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [Column("original_name")]
        public string? OriginalName { get; set; }

        [Column("captured_at")]
        public DateTime CapturedAt { get; set; } = DateTime.Now;
    }
}