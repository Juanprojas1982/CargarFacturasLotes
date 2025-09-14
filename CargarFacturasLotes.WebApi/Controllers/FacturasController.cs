using CargarFacturasLotes.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CargarFacturasLotes.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacturasController : ControllerBase
    {
        [HttpPost("Anular")]
        public IActionResult Anular([FromBody] RequestDto request)
        {
            return Ok($"IdAdmision: {request.IdAdmision} - SedeId: {request.SedeId}");
        }

        [HttpPost("Numerar")]
        public IActionResult Numerar([FromBody] RequestDto request)
        {
            return Ok($"IdAdmision: {request.IdAdmision} - SedeId: {request.SedeId}");
        }
    }
}
