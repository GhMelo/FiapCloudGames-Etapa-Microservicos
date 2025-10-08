namespace Domain.Entities
{
    public class UsuarioBiblioteca : EntityBase
    {
        public int UsuarioId { get; set; }
        public Guid JogoExternalId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;

        public UsuarioBiblioteca(int usuarioId, Guid jogoExternalId)
        {
            UsuarioId = usuarioId;
            JogoExternalId = jogoExternalId;
        }
    }
}