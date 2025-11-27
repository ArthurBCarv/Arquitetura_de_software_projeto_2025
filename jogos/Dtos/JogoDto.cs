namespace Jogos.Dtos
{
    public class JogoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public decimal Preco { get; set; }
        public string Desenvolvedor { get; set; } = "";
        public DateTime DataLancamento { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
