using System.ComponentModel.DataAnnotations;

namespace Application.Input.JogoInput
{
    public class JogoAlteracaoInput
    {
        [Required(ErrorMessage = "Id é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Produtora é obrigatório.")]
        public required string Produtora { get; set; }

        [Required(ErrorMessage = "Genero é obrigatório.")]
        public required string Genero { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório.")]
        public decimal Valor { get; set; }
    }
}
