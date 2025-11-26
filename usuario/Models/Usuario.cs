namespace Usuarios.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string SenhaHash { get; set; } = "";
        public int Pontos { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;
    }
}
