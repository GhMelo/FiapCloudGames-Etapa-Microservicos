using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json;

namespace Domain.Events
{
    public class DomainEvent
    {
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string EntityType { get; private set; }
        public string EntityId { get; private set; }
        public string Operation { get; private set; }
        public string Data { get; private set; } 
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
        public string? CorrelationId { get; private set; }

        public DomainEvent(object entity, string operation, string? correlationId = null)
        {
            EntityType = entity.GetType().Name;
            EntityId = GetEntityId(entity);
            Operation = operation;
            Data = JsonSerializer.Serialize(entity);
            CorrelationId = correlationId;
        }

        private static string GetEntityId(object entity)
        {
            var prop = entity.GetType().GetProperty("Id");
            return prop?.GetValue(entity)?.ToString() ?? Guid.NewGuid().ToString();
        }
    }
}
