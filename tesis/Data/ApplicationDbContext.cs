using Microsoft.EntityFrameworkCore;
using tesis.Models;

namespace tesis.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<CommandQueue> CommandQueue { get; set; }
        public DbSet<ExfiltratedPhoto> ExfiltratedPhotos { get; set; }

        // Si necesitas configuración extra
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aquí se pueden forzar relaciones si fallan las automáticas
        }
    }
}