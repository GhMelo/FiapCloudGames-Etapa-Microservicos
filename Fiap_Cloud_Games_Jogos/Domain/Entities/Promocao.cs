namespace Domain.Entities
{
    public class Promocao : EntityBase
    {
        public int JogoId { get; set; }
        public Guid ExternalId { get; set; }
        public string NomePromocao { get; set; }
        public decimal PorcentagemDesconto { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int Status { get; set; }
        public virtual Jogo JogoPromocao { get; set; } = null!;

        public Promocao(int jogoId, string nomePromocao, decimal porcentagemDesconto, DateTime dataInicio, DateTime dataFim, int status)
        {
            JogoId = jogoId;
            NomePromocao = nomePromocao;
            PorcentagemDesconto = porcentagemDesconto;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Status = status;
        }
    }
}
