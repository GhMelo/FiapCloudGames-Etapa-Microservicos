namespace Domain.Entities
{
    public class Pagamento : EntityBase
    {
        public int CompraId { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime DataPagamento { get; set; } = DateTime.UtcNow;
        public string Metodo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public virtual Compra Compra { get; set; } = null!;

        public Pagamento(int compraId, decimal valorPago, string metodo)
        {
            CompraId = compraId;
            ValorPago = valorPago;
            Metodo = metodo;
        }
    }
}
