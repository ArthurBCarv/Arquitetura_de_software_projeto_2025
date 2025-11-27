namespace Compras.Dtos
{
    public class CompraDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int JogoId { get; set; }
        public decimal PrecoCompra { get; set; }
        public DateTime DataCompra { get; set; }
        public string Status { get; set; } = "";
        public JogoInfoDto? JogoInfo { get; set; }
        public UsuarioInfoDto? UsuarioInfo { get; set; }
    }

    public class JogoInfoDto
    {
        public string Titulo { get; set; } = "";
        public string Desenvolvedor { get; set; } = "";
    }

    public class UsuarioInfoDto
    {
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
