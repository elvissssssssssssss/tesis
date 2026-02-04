using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tesis.Models
{
    [Table("command_queue")]
    public class CommandQueue
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("device_id")]
        [ForeignKey("Device")]
        public string DeviceId { get; set; } = string.Empty;
        public Device? Device { get; set; }

        [Column("command_type")]
        public string CommandType { get; set; } = string.Empty; // Ej: 'TAKE_PHOTO'

        [Column("parameters")]
        public string? Parameters { get; set; }

        [Column("status")]
        public string Status { get; set; } = "PENDING";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("executed_at")]
        public DateTime? ExecutedAt { get; set; }
    }
}