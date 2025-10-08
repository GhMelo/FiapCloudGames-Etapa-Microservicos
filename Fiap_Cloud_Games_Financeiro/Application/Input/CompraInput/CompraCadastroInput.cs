using System.ComponentModel.DataAnnotations;

namespace Application.Input.CompraInput
{
    public class CompraCadastroInput
    {
        [Required(ErrorMessage = "Usuário é obrigatório.")]
        public Guid UsuarioExternalId { get; set; }

        [Required(ErrorMessage = "Jogo é obrigatório.")]
        public Guid JogoExternalId { get; set; }

        public Guid? PromocaoExternalId { get; set; }

        [Required(ErrorMessage = "Valor da compra é obrigatório.")]
        public decimal ValorCompra { get; set; }

        [Required(ErrorMessage = "Status é obrigatório.")]
        public string Status { get; set; } = string.Empty;
    }
}
