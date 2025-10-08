using Domain.Events;

namespace Domain.Interfaces.IRepository
{
    public interface IEventStoreRepository
    {
        Task SalvarEventoAsync(DomainEvent evento);
    }
}
