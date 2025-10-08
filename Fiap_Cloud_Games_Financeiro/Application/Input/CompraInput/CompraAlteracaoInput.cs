using System.ComponentModel.DataAnnotations;

namespace Application.Input.CompraInput
{
    public class CompraAlteracaoInput
    {
        [Required(ErrorMessage = "Id é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Valor da compra é obrigatório.")]
        public decimal ValorCompra { get; set; }

        [Required(ErrorMessage = "Status é obrigatório.")]
        public string Status { get; set; } = string.Empty;
    }
}
