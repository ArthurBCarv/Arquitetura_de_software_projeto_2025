using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Jogos.Data;

namespace jogos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatusController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { 
            message = "Serviço de Jogos está funcionando!",
            status = "Healthy",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("health")]
    public async Task<IActionResult> HealthCheck()
    {
        try
        {
            // Verifica se consegue se conectar com o banco
            var canConnect = await _context.Database.CanConnectAsync();
            
            return Ok(new {
                status = "Healthy",
                database = canConnect ? "Connected" : "Disconnected",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                status = "Unhealthy",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}