namespace Application.Dtos
{
    public class PromocaoDto
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public int JogoId { get; set; }
        public string? NomePromocao { get; set; }
        public decimal PorcentagemDesconto { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int Status { get; set; }
        public JogoDto? JogoPromocao { get; set; }
    }
}
