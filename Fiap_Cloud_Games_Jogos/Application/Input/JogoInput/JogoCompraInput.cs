namespace Application.Input.JogoInput
{
    public class JogoCompraInput
    {
        public Guid UsuarioExternalId { get; set; }
        public Guid JogoExternalId { get; set; }
        public Guid PromocaoExternalId { get; set; }
        public decimal ValorCompra { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
