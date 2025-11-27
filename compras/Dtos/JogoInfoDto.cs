namespace Compras.Dtos
{
    public class JogoInfoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public decimal Preco { get; set; }
        public bool Disponivel { get; set; }
    }
}
