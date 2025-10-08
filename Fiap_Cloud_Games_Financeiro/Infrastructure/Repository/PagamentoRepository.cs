using Domain.Entities;
using Domain.Interfaces.IRepository;

namespace Infrastructure.Repository
{
    public class PagamentoRepository : EFRepository<Pagamento>, IPagamentoRepository
    {
        public PagamentoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
