using Domain.Entities;

public class Compra : EntityBase
{
    public Guid UsuarioExternalId { get; set; }
    public Guid JogoExternalId { get; set; }
    public Guid? PromocaoExternalId { get; set; }
    public decimal ValorCompra { get; set; }
    public string Status { get; set; } = string.Empty;

    public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();

    public Compra(Guid usuarioExternalId, Guid jogoExternalId, decimal valorCompra, string status, Guid? promocaoExternalId = null)
    {
        UsuarioExternalId = usuarioExternalId;
        JogoExternalId = jogoExternalId;
        ValorCompra = valorCompra;
        Status = status;
        PromocaoExternalId = promocaoExternalId;
    }
}