using Domain.Entities;
using Domain.Interfaces.IRepository;

namespace Infrastructure.Repository
{
    public class UsuarioBibliotecaRepository : EFRepository<UsuarioBiblioteca>, IUsuarioBibliotecaRepository
    {
        public UsuarioBibliotecaRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

