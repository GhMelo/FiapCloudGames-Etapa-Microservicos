namespace Application.Dtos
{
    public class JogoDto
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Nome { get; set; } = null!;
        public string Produtora { get; set; } = null!;
        public string? Genero { get; set; } = null!;
        public decimal Valor { get; set; }
        public ICollection<PromocaoDto>? PromocoesAderidas { get; set; } = new List<PromocaoDto>();
    }
}
