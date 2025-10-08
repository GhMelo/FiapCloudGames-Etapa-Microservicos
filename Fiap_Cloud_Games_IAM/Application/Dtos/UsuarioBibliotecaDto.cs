namespace Application.Dtos
{
    public class UsuarioBibliotecaDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Guid JogoExternalId { get; set; }
        public DateTime DataCriacao { get; set; }
        public UsuarioDto? UsuarioBiblioteca { get; set; }
    }
}
