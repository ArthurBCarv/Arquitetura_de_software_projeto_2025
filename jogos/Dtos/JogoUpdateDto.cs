namespace Jogos.Dtos
{
    public class JogoUpdateDto
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public decimal? Preco { get; set; }
        public bool? Disponivel { get; set; }
    }
}
