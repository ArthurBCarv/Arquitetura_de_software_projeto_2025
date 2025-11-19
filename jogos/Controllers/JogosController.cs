using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jogos.Data;
using jogos.Models;

namespace jogos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<JogosController> _logger;

    public JogosController(AppDbContext context, ILogger<JogosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Jogo>>> GetJogos()
    {
        _logger.LogInformation("Buscando todos os jogos ativos");
        return await _context.Jogos.Where(j => j.Ativo).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Jogo>> GetJogo(int id)
    {
        _logger.LogInformation("Buscando jogo com ID: {JogoId}", id);
        
        var jogo = await _context.Jogos.FindAsync(id);

        if (jogo == null || !jogo.Ativo)
        {
            _logger.LogWarning("Jogo com ID {JogoId} não encontrado", id);
            return NotFound(new { message = "Jogo não encontrado" });
        }

        return jogo;
    }

    [HttpPost]
    public async Task<ActionResult<Jogo>> PostJogo(JogoCreateDto jogoDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Tentativa de criar jogo com dados inválidos");
            return BadRequest(ModelState);
        }

        var jogo = new Jogo
        {
            Titulo = jogoDto.Titulo,
            Descricao = jogoDto.Descricao,
            Preco = jogoDto.Preco,
            Desenvolvedor = jogoDto.Desenvolvedor,
            DataLancamento = jogoDto.DataLancamento,
            Ativo = true
        };

        _context.Jogos.Add(jogo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Jogo criado com ID: {JogoId}", jogo.Id);
        return CreatedAtAction(nameof(GetJogo), new { id = jogo.Id }, jogo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutJogo(int id, JogoUpdateDto jogoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id <= 0)
        {
            return BadRequest(new { message = "ID do jogo inválido" });
        }

        var jogoExistente = await _context.Jogos.FindAsync(id);
        if (jogoExistente == null || !jogoExistente.Ativo)
        {
            return NotFound(new { message = "Jogo não encontrado" });
        }

        // Atualiza apenas os campos permitidos
        jogoExistente.Titulo = jogoDto.Titulo;
        jogoExistente.Descricao = jogoDto.Descricao;
        jogoExistente.Preco = jogoDto.Preco;
        jogoExistente.Desenvolvedor = jogoDto.Desenvolvedor;
        jogoExistente.DataLancamento = jogoDto.DataLancamento;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Jogo com ID {JogoId} atualizado com sucesso", id);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!JogoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJogo(int id)
    {
        var jogo = await _context.Jogos.FindAsync(id);
        if (jogo == null)
        {
            _logger.LogWarning("Tentativa de excluir jogo inexistente com ID: {JogoId}", id);
            return NotFound(new { message = "Jogo não encontrado" });
        }

        jogo.Ativo = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Jogo com ID {JogoId} desativado com sucesso", id);
        return NoContent();
    }

    private bool JogoExists(int id)
    {
        return _context.Jogos.Any(e => e.Id == id && e.Ativo);
    }
}