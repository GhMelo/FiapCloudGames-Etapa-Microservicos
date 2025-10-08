using System.ComponentModel.DataAnnotations;

namespace Application.Input.UsuarioBibliotecaInput
{
    public class UsuarioBibliotecaCadastroInput
    {
        [Required(ErrorMessage = "UsuarioId é obrigatório.")]
        public Guid UsuarioExternalId { get; set; }

        [Required(ErrorMessage = "JogoExternalId é obrigatório.")]
        public Guid JogoExternalId { get; set; }
    }
}
