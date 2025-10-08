using Domain.Entities;

namespace Application.Dtos
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public DateTime DataCriacao { get; set; }
        public TipoUsuario Tipo { get; set; }
        public IEnumerable<UsuarioBibliotecaDto>? Biblioteca { get; set; } = new List<UsuarioBibliotecaDto>();
    }
}
