using Domain.Events;
using Domain.Interfaces.IRepository;
using MongoDB.Driver;

namespace Infrastructure.Repository
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<DomainEvent> _collection;

        public EventStoreRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<DomainEvent>("DomainEvent");
        }

        public async Task SalvarEventoAsync(DomainEvent evento)
        {
            await _collection.InsertOneAsync(evento);
        }
    }
}
