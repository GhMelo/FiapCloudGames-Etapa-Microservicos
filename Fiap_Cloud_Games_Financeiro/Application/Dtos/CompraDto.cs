namespace Application.Dtos
{
    public class CompraDto
    {
        public int Id { get; set; }
        public Guid UsuarioExternalId { get; set; }
        public Guid JogoExternalId { get; set; }
        public Guid? PromocaoExternalId { get; set; }
        public decimal ValorCompra { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }

        public IEnumerable<PagamentoDto>? Pagamentos { get; set; } = new List<PagamentoDto>();
    }
}
