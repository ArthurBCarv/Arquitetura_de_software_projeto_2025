using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Compras.Data;
using Compras.Dtos;
using Compras.Models;
using Compras.Services;

namespace Compras.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JogosService _jogosService;
        private readonly UsuariosService _usuariosService;

        public ComprasController(
            AppDbContext context, 
            JogosService jogosService,
            UsuariosService usuariosService)
        {
            _context = context;
            _jogosService = jogosService;
            _usuariosService = usuariosService;
        }

        [HttpPost]
        public async Task<ActionResult<CompraDto>> Create(CompraCreateDto dto)
        {
            var usuario = await _usuariosService.GetUsuarioByIdAsync(dto.UsuarioId);
            if (usuario == null)
                return BadRequest("Usuário não encontrado.");

            if (!usuario.Ativo)
                return BadRequest("Usuário inativo.");

            var jogo = await _jogosService.GetJogoByIdAsync(dto.JogoId);
            if (jogo == null)
                return BadRequest("Jogo não encontrado.");

            if (!jogo.Ativo)
                return BadRequest("Jogo não disponível para compra.");

            var jaComprou = await _context.Compras
                .AnyAsync(c => c.UsuarioId == dto.UsuarioId && c.JogoId == dto.JogoId);
            
            if (jaComprou)
                return BadRequest("Usuário já possui este jogo.");

            var compra = new Compra
            {
                UsuarioId = dto.UsuarioId,
                JogoId = dto.JogoId,
                PrecoCompra = jogo.Preco,
                DataCompra = DateTime.UtcNow,
                Status = "Concluida"
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            await _usuariosService.AdicionarJogoBibliotecaAsync(dto.UsuarioId, dto.JogoId);

            var compraDto = await MapToDtoAsync(compra);
            return CreatedAtAction(nameof(GetById), new { id = compra.Id }, compraDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDto>>> GetAll([FromQuery] int? usuarioId = null)
        {
            var query = _context.Compras.AsQueryable();

            if (usuarioId.HasValue)
                query = query.Where(c => c.UsuarioId == usuarioId.Value);

            var compras = await query.OrderByDescending(c => c.DataCompra).ToListAsync();
            
            var comprasDto = new List<CompraDto>();
            foreach (var compra in compras)
            {
                comprasDto.Add(await MapToDtoAsync(compra));
            }

            return Ok(comprasDto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CompraDto>> GetById(int id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
                return NotFound();

            var compraDto = await MapToDtoAsync(compra);
            return Ok(compraDto);
        }

        [HttpGet("usuario/{usuarioId:int}")]
        public async Task<ActionResult<IEnumerable<CompraDto>>> GetByUsuario(int usuarioId)
        {
            var compras = await _context.Compras
                .Where(c => c.UsuarioId == usuarioId)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

            var comprasDto = new List<CompraDto>();
            foreach (var compra in compras)
            {
                comprasDto.Add(await MapToDtoAsync(compra));
            }

            return Ok(comprasDto);
        }

        private async Task<CompraDto> MapToDtoAsync(Compra compra)
        {
            var dto = new CompraDto
            {
                Id = compra.Id,
                UsuarioId = compra.UsuarioId,
                JogoId = compra.JogoId,
                PrecoCompra = compra.PrecoCompra,
                DataCompra = compra.DataCompra,
                Status = compra.Status
            };

            var jogo = await _jogosService.GetJogoByIdAsync(compra.JogoId);
            if (jogo != null)
            {
                dto.JogoInfo = new JogoInfoDto
                {
                    Titulo = jogo.Titulo,
                    Desenvolvedor = jogo.Desenvolvedor
                };
            }

            var usuario = await _usuariosService.GetUsuarioByIdAsync(compra.UsuarioId);
            if (usuario != null)
            {
                dto.UsuarioInfo = new UsuarioInfoDto
                {
                    Nome = usuario.Nome,
                    Email = usuario.Email
                };
            }

            return dto;
        }
    }
}
