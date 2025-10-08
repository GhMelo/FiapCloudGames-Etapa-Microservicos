using Application.Validations.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Application.Input.PromocaoInput
{
    public class PromocaoCadastroInput
    {
        [Required(ErrorMessage = "JogoId é obrigatório.")]
        public required int JogoId { get; set; }

        [Required(ErrorMessage = "NomePromocao é obrigatório.")]
        public required string NomePromocao { get; set; }

        [Required(ErrorMessage = "Porcentagem é obrigatória.")]
        [PorcentagemAttribute]
        public required decimal PorcentagemDesconto { get; set; }

        [Required(ErrorMessage = "DataInicio é obrigatório.")]
        public required DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "DataFim é obrigatório.")]
        public required DateTime DataFim { get; set; }

        [Required(ErrorMessage = "Status é obrigatório.")]
        public required int Status { get; set; }
    }
}
