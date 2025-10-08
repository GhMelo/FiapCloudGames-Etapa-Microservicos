namespace Domain.Interfaces.IRepository
{
    public interface IMongoRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task InsertOneAsync(T document);
        Task UpdateAsync(string id, T document);
        Task DeleteAsync(string id);
    }
}