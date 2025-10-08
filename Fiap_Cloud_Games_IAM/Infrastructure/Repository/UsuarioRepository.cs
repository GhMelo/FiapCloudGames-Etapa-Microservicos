using Domain.Entities;
using Domain.Interfaces.IRepository;

namespace Infrastructure.Repository
{
    public class UsuarioRepository : EFRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
