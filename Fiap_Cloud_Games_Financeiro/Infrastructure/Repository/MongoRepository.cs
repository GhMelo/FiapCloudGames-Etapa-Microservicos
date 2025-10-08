using Domain.Entity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Domain.Interfaces.IRepository;

namespace Infrastructure.Repository
{
    public class MongoRepository<T> : IMongoRepository<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.Database);
            _collection = database.GetCollection<T>(typeof(T).Name);
        }

        public async Task<List<T>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<T> GetByIdAsync(string id) =>
            await _collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstOrDefaultAsync();

        public async Task InsertOneAsync(T document) =>
            await _collection.InsertOneAsync(document);

        public async Task UpdateAsync(string id, T document) =>
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", id), document);

        public async Task DeleteAsync(string id) =>
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", id));
    }
}