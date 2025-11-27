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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoDto>>> GetAll()
        {
            var jogos = await _context.Jogos
                .Where(j => j.Disponivel)
                .Select(j => new JogoDto
                {
                    Id = j.Id,
                    Titulo = j.Titulo,
                    Descricao = j.Descricao,
                    Preco = j.Preco,
                    Desenvolvedor = j.Desenvolvedor,
                    DataLancamento = j.DataLancamento,
                    Disponivel = j.Disponivel,
                    DataCriacao = j.DataCriacao
                })
                .ToListAsync();

            return Ok(jogos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<JogoDto>> GetById(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
                return NotFound();

            var jogoDto = new JogoDto
            {
                Id = jogo.Id,
                Titulo = jogo.Titulo,
                Descricao = jogo.Descricao,
                Preco = jogo.Preco,
                Desenvolvedor = jogo.Desenvolvedor,
                DataLancamento = jogo.DataLancamento,
                Disponivel = jogo.Disponivel,
                DataCriacao = jogo.DataCriacao
            };

            return Ok(jogoDto);
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
                Disponivel = true,
                DataCriacao = DateTime.UtcNow
            };

            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();

            var jogoDto = new JogoDto
            {
                Id = jogo.Id,
                Titulo = jogo.Titulo,
                Descricao = jogo.Descricao,
                Preco = jogo.Preco,
                Desenvolvedor = jogo.Desenvolvedor,
                DataLancamento = jogo.DataLancamento,
                Disponivel = jogo.Disponivel,
                DataCriacao = jogo.DataCriacao
            };

            return CreatedAtAction(nameof(GetById), new { id = jogo.Id }, jogoDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<JogoDto>> Update(int id, JogoUpdateDto dto)
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

            if (dto.Disponivel.HasValue)
                jogo.Disponivel = dto.Disponivel.Value;

            await _context.SaveChangesAsync();

            var jogoDto = new JogoDto
            {
                Id = jogo.Id,
                Titulo = jogo.Titulo,
                Descricao = jogo.Descricao,
                Preco = jogo.Preco,
                Desenvolvedor = jogo.Desenvolvedor,
                DataLancamento = jogo.DataLancamento,
                Disponivel = jogo.Disponivel,
                DataCriacao = jogo.DataCriacao
            };

            return Ok(jogoDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
                return NotFound();

            jogo.Disponivel = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
