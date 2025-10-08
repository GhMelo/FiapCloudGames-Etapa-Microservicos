using Domain.Entities;
using Domain.Interfaces.IRepository;

namespace Infrastructure.Repository
{
    public class PromocaoRepository : EFRepository<Promocao>, IPromocaoRepository
    {
        public PromocaoRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
