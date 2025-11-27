namespace Compras.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int JogoId { get; set; }
        public decimal PrecoCompra { get; set; }
        public DateTime DataCompra { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Concluida";
    }
}
