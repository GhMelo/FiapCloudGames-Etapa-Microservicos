using System.ComponentModel.DataAnnotations;

namespace Application.Input.JogoInput
{
    public class JogoCadastroInput
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Produtora é obrigatório.")]
        public required string Produtora { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Genero é obrigatório.")]
        public required string Genero { get; set; }
    }
}
