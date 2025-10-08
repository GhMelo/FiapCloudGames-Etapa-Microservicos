using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Input.UsuarioInput
{
    public class UsuarioAlteracaoInput
    {
        [Required(ErrorMessage = "Id é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public required string Email { get; set; }

        public string? Senha { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório.")]
        public TipoUsuario Tipo { get; set; }
    }
}
