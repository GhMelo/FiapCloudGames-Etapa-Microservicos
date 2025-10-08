using Domain.Interfaces.IRepository;

namespace Infrastructure.Repository
{
    public class CompraRepository : EFRepository<Compra>, ICompraRepository
    {
        public CompraRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
