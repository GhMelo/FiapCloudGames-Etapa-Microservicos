using System.ComponentModel.DataAnnotations;

namespace Application.Input.UsuarioBibliotecaInput
{
    public class UsuarioBibliotecaAlteracaoInput
    {
        [Required(ErrorMessage = "Id é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "UsuarioId é obrigatório.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "JogoExternalId é obrigatório.")]
        public Guid JogoExternalId { get; set; }
    }
}
