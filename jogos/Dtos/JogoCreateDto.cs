namespace Jogos.Dtos
{
    public class JogoCreateDto
    {
        public string Titulo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public decimal Preco { get; set; }
        public string Desenvolvedor { get; set; } = "";
        public DateTime DataLancamento { get; set; }
    }
}
