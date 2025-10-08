namespace Domain.Entities
{
    public enum TipoUsuario
    {
        Padrao = 0,
        Administrador = 1
    }
    public class Usuario : EntityBase
    {
        public Guid ExternalId { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public TipoUsuario Tipo { get; set; }
        public virtual ICollection<UsuarioBiblioteca> Biblioteca { get; set; } = new List<UsuarioBiblioteca>();
        public Usuario(string email, string nome, string senha, TipoUsuario tipo)
        {
            ExternalId = Guid.NewGuid();
            Email = email;
            Nome = nome;
            Senha = senha;
            Tipo = tipo;
        }
    }
}