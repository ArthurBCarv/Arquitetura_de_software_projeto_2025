namespace Jogos.Models
{
    public class Jogo
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public decimal Preco { get; set; }
        public string Desenvolvedor { get; set; } = "";
        public DateTime DataLancamento { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
