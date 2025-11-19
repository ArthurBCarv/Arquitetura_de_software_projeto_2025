using System.ComponentModel.DataAnnotations;

namespace jogos.Models;

public class JogoUpdateDto
{
    [Required(ErrorMessage = "O título do jogo é obrigatório")]
    [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [Range(0, 999.99, ErrorMessage = "O preço deve estar entre 0 e 999.99")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "O desenvolvedor é obrigatório")]
    [StringLength(100, ErrorMessage = "O desenvolvedor deve ter no máximo 100 caracteres")]
    public string Desenvolvedor { get; set; } = string.Empty;

    public DateTime DataLancamento { get; set; }
}