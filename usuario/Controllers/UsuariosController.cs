using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Usuarios.Data;
using Usuarios.Dtos;
using Usuarios.Models;

namespace Usuarios.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        private static string GerarHashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UsuarioProfileDto>> Register(UsuarioRegisterDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var existe = await _context.Usuarios.AnyAsync(u => u.Email == email);
            if (existe)
                return BadRequest("Email já cadastrado.");

            var usuario = new Usuario
            {
                Nome = dto.Nome.Trim(),
                Email = email,
                SenhaHash = GerarHashSenha(dto.Senha),
                Pontos = 0,
                DataCriacao = DateTime.UtcNow,
                Ativo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var perfil = new UsuarioProfileDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Pontos = usuario.Pontos,
                Ativo = usuario.Ativo,
                DataCriacao = usuario.DataCriacao
            };

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, perfil);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioProfileDto>> Login(UsuarioLoginDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null)
                return BadRequest("Credenciais inválidas.");

            var hash = GerarHashSenha(dto.Senha);
            if (usuario.SenhaHash != hash)
                return BadRequest("Credenciais inválidas.");

            if (!usuario.Ativo)
                return BadRequest("Usuário inativo.");

            var perfil = new UsuarioProfileDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Pontos = usuario.Pontos,
                Ativo = usuario.Ativo,
                DataCriacao = usuario.DataCriacao
            };

            return Ok(perfil);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioProfileDto>> GetById(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            var perfil = new UsuarioProfileDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Pontos = usuario.Pontos,
                Ativo = usuario.Ativo,
                DataCriacao = usuario.DataCriacao
            };

            return Ok(perfil);
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UsuarioUpdateStatusDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Ativo = dto.Ativo;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
