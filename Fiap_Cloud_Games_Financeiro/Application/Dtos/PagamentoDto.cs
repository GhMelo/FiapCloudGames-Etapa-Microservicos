namespace Application.Dtos
{
    public class PagamentoDto
    {
        public int Id { get; set; }
        public int CompraId { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
        public string Metodo { get; set; } = string.Empty;
    }
}
