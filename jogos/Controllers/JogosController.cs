using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Jogos.Data;
using Jogos.Dtos;
using Jogos.Models;

namespace Jogos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JogosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JogosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<JogoDto>> Create(JogoCreateDto dto)
        {
            var jogo = new Jogo
            {
                Titulo = dto.Titulo.Trim(),
                Descricao = dto.Descricao.Trim(),
                Preco = dto.Preco,
                Desenvolvedor = dto.Desenvolvedor.Trim(),
                DataLancamento = dto.DataLancamento,
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();

            var jogoDto = MapToDto(jogo);
            return CreatedAtAction(nameof(GetById), new { id = jogo.Id }, jogoDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoDto>>> GetAll([FromQuery] bool? ativo = null)
        {
            var query = _context.Jogos.AsQueryable();

            if (ativo.HasValue)
                query = query.Where(j => j.Ativo == ativo.Value);

            var jogos = await query.OrderByDescending(j => j.DataCriacao).ToListAsync();
            return Ok(jogos.Select(MapToDto));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<JogoDto>> GetById(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
                return NotFound();

            return Ok(MapToDto(jogo));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, JogoUpdateDto dto)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Titulo))
                jogo.Titulo = dto.Titulo.Trim();
            
            if (!string.IsNullOrWhiteSpace(dto.Descricao))
                jogo.Descricao = dto.Descricao.Trim();
            
            if (dto.Preco.HasValue)
                jogo.Preco = dto.Preco.Value;
            
            if (!string.IsNullOrWhiteSpace(dto.Desenvolvedor))
                jogo.Desenvolvedor = dto.Desenvolvedor.Trim();
            
            if (dto.DataLancamento.HasValue)
                jogo.DataLancamento = dto.DataLancamento.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, JogoUpdateStatusDto dto)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
                return NotFound();

            jogo.Ativo = dto.Ativo;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
                return NotFound();

            _context.Jogos.Remove(jogo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static JogoDto MapToDto(Jogo jogo)
        {
            return new JogoDto
            {
                Id = jogo.Id,
                Titulo = jogo.Titulo,
                Descricao = jogo.Descricao,
                Preco = jogo.Preco,
                Desenvolvedor = jogo.Desenvolvedor,
                DataLancamento = jogo.DataLancamento,
                Ativo = jogo.Ativo,
                DataCriacao = jogo.DataCriacao
            };
        }
    }
}
