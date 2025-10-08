using System.ComponentModel.DataAnnotations;

namespace Application.Input.PagamentoInput
{
    public class PagamentoAlteracaoInput
    {
        [Required(ErrorMessage = "Id é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Valor pago é obrigatório.")]
        public decimal ValorPago { get; set; }

        [Required(ErrorMessage = "Método de pagamento é obrigatório.")]
        public string Metodo { get; set; } = string.Empty;
    }
}
