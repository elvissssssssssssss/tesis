using Microsoft.AspNetCore.Mvc;
using tesis.Services; // Asegúrate de importar tu servicio

namespace tesis.Controllers
{
    [Route("api/[controller]")] // Esto define la base como "api/Admin"
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly C2Service _c2Service;

        public AdminController(C2Service c2Service)
        {
            _c2Service = c2Service;
        }

        // 👇 ESTO ES LO QUE TE FALTA O ESTÁ MAL ESCRITO 👇
        [HttpGet("devices")]
        public IActionResult GetDevices()
        {
            var devices = _c2Service.GetAllDevices();
            return Ok(devices);
        }
        // 👆 ------------------------------------------ 👆

        [HttpPost("send-command")]
        public async Task<IActionResult> SendCommand([FromBody] DTOs.CommandDto dto)
        {
            var success = await _c2Service.QueueCommandAsync(dto.DeviceId, dto.Command);
            if (!success) return NotFound("Dispositivo no encontrado");
            return Ok(new { message = "Orden enviada" });
        }
    }
}