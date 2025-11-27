namespace Compras.Dtos
{
    public class CompraDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public int JogoId { get; set; }
        public string? JogoTitulo { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime DataCompra { get; set; }
        public string Status { get; set; } = "";
    }
}
