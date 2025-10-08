using Domain.Entities;
using Domain.Interfaces.IRepository;

namespace Infrastructure.Repository
{
    public class JogoRepository : EFRepository<Jogo>, IJogoRepository
    {
        public JogoRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
