namespace Domain.Entities
{
    public class Jogo : EntityBase
    {
        public Guid ExternalId { get; set; }
        public string Nome { get; set; }
        public string Produtora { get; set; }
        public string? Genero { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public virtual ICollection<Promocao> PromocoesAderidas { get; set; } = new List<Promocao>();
        public Jogo(string nome, string produtora, decimal valor, string genero)
        {
            Nome = nome;
            Produtora = produtora;
            Valor = valor;
            ExternalId = Guid.NewGuid();
            Genero = genero;
        }
    }
}
