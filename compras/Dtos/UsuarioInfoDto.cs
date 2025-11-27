namespace Compras.Dtos
{
    public class UsuarioInfoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public List<int> BibliotecaJogos { get; set; } = new List<int>();
    }
}
