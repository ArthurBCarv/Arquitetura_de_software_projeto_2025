using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Compras.Data;
using Compras.Dtos;
using Compras.Models;
using System.Text.Json;

namespace Compras.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public ComprasController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDto>>> GetAll()
        {
            var compras = await _context.Compras.ToListAsync();
            var comprasDto = new List<CompraDto>();

            foreach (var compra in compras)
            {
                var compraDto = new CompraDto
                {
                    Id = compra.Id,
                    UsuarioId = compra.UsuarioId,
                    JogoId = compra.JogoId,
                    ValorPago = compra.ValorPago,
                    DataCompra = compra.DataCompra,
                    Status = compra.Status
                };

                try
                {
                    var usuarioResponse = await _httpClient.GetAsync($"http://localhost:5000/api/usuarios/{compra.UsuarioId}");
                    if (usuarioResponse.IsSuccessStatusCode)
                    {
                        var usuarioJson = await usuarioResponse.Content.ReadAsStringAsync();
                        var usuario = JsonSerializer.Deserialize<UsuarioInfoDto>(usuarioJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (usuario != null)
                            compraDto.UsuarioNome = usuario.Nome;
                    }
                }
                catch { }

                try
                {
                    var jogoResponse = await _httpClient.GetAsync($"http://localhost:5001/api/jogos/{compra.JogoId}");
                    if (jogoResponse.IsSuccessStatusCode)
                    {
                        var jogoJson = await jogoResponse.Content.ReadAsStringAsync();
                        var jogo = JsonSerializer.Deserialize<JogoInfoDto>(jogoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (jogo != null)
                            compraDto.JogoTitulo = jogo.Titulo;
                    }
                }
                catch { }

                comprasDto.Add(compraDto);
            }

            return Ok(comprasDto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CompraDto>> GetById(int id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
                return NotFound();

            var compraDto = new CompraDto
            {
                Id = compra.Id,
                UsuarioId = compra.UsuarioId,
                JogoId = compra.JogoId,
                ValorPago = compra.ValorPago,
                DataCompra = compra.DataCompra,
                Status = compra.Status
            };

            try
            {
                var usuarioResponse = await _httpClient.GetAsync($"http://localhost:5000/api/usuarios/{compra.UsuarioId}");
                if (usuarioResponse.IsSuccessStatusCode)
                {
                    var usuarioJson = await usuarioResponse.Content.ReadAsStringAsync();
                    var usuario = JsonSerializer.Deserialize<UsuarioInfoDto>(usuarioJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (usuario != null)
                        compraDto.UsuarioNome = usuario.Nome;
                }
            }
            catch { }

            try
            {
                var jogoResponse = await _httpClient.GetAsync($"http://localhost:5001/api/jogos/{compra.JogoId}");
                if (jogoResponse.IsSuccessStatusCode)
                {
                    var jogoJson = await jogoResponse.Content.ReadAsStringAsync();
                    var jogo = JsonSerializer.Deserialize<JogoInfoDto>(jogoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (jogo != null)
                        compraDto.JogoTitulo = jogo.Titulo;
                }
            }
            catch { }

            return Ok(compraDto);
        }

        [HttpGet("usuario/{usuarioId:int}")]
        public async Task<ActionResult<IEnumerable<CompraDto>>> GetByUsuario(int usuarioId)
        {
            var compras = await _context.Compras
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            var comprasDto = new List<CompraDto>();

            foreach (var compra in compras)
            {
                var compraDto = new CompraDto
                {
                    Id = compra.Id,
                    UsuarioId = compra.UsuarioId,
                    JogoId = compra.JogoId,
                    ValorPago = compra.ValorPago,
                    DataCompra = compra.DataCompra,
                    Status = compra.Status
                };

                try
                {
                    var jogoResponse = await _httpClient.GetAsync($"http://localhost:5001/api/jogos/{compra.JogoId}");
                    if (jogoResponse.IsSuccessStatusCode)
                    {
                        var jogoJson = await jogoResponse.Content.ReadAsStringAsync();
                        var jogo = JsonSerializer.Deserialize<JogoInfoDto>(jogoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (jogo != null)
                            compraDto.JogoTitulo = jogo.Titulo;
                    }
                }
                catch { }

                comprasDto.Add(compraDto);
            }

            return Ok(comprasDto);
        }

        [HttpPost]
        public async Task<ActionResult<CompraDto>> Create(CompraCreateDto dto)
        {
            try
            {
                var usuarioResponse = await _httpClient.GetAsync($"http://localhost:5000/api/usuarios/{dto.UsuarioId}");
                if (!usuarioResponse.IsSuccessStatusCode)
                    return BadRequest("Usuário não encontrado.");
            }
            catch
            {
                return BadRequest("Erro ao buscar usuário.");
            }

            JogoInfoDto? jogo = null;
            try
            {
                var jogoResponse = await _httpClient.GetAsync($"http://localhost:5001/api/jogos/{dto.JogoId}");
                if (!jogoResponse.IsSuccessStatusCode)
                    return BadRequest("Jogo não encontrado.");

                var jogoJson = await jogoResponse.Content.ReadAsStringAsync();
                jogo = JsonSerializer.Deserialize<JogoInfoDto>(jogoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (jogo == null || !jogo.Disponivel)
                    return BadRequest("Jogo não disponível para compra.");
            }
            catch
            {
                return BadRequest("Erro ao buscar jogo.");
            }

            var compraExistente = await _context.Compras
                .AnyAsync(c => c.UsuarioId == dto.UsuarioId && c.JogoId == dto.JogoId);

            if (compraExistente)
                return BadRequest("Usuário já possui este jogo.");

            var compra = new Compra
            {
                UsuarioId = dto.UsuarioId,
                JogoId = dto.JogoId,
                ValorPago = jogo.Preco,
                DataCompra = DateTime.UtcNow,
                Status = "Concluída"
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            try
            {
                var usuarioResponse = await _httpClient.GetAsync($"http://localhost:5000/api/usuarios/{dto.UsuarioId}");
                if (usuarioResponse.IsSuccessStatusCode)
                {
                    var usuarioJson = await usuarioResponse.Content.ReadAsStringAsync();
                    var usuario = JsonSerializer.Deserialize<UsuarioInfoDto>(usuarioJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (usuario != null)
                    {
                        if (!usuario.BibliotecaJogos.Contains(dto.JogoId))
                        {
                            usuario.BibliotecaJogos.Add(dto.JogoId);

                            var updateContent = new StringContent(
                                JsonSerializer.Serialize(new { bibliotecaJogos = usuario.BibliotecaJogos }),
                                System.Text.Encoding.UTF8,
                                "application/json"
                            );

                            await _httpClient.PutAsync($"http://localhost:5000/api/usuarios/{dto.UsuarioId}/biblioteca", updateContent);
                        }
                    }
                }
            }
            catch { }

            var compraDto = new CompraDto
            {
                Id = compra.Id,
                UsuarioId = compra.UsuarioId,
                JogoId = compra.JogoId,
                JogoTitulo = jogo.Titulo,
                ValorPago = compra.ValorPago,
                DataCompra = compra.DataCompra,
                Status = compra.Status
            };

            return CreatedAtAction(nameof(GetById), new { id = compra.Id }, compraDto);
        }
    }
}
