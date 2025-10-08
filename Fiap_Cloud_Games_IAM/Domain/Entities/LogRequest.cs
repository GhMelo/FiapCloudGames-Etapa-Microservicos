using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entity
{
    public class LogRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
        public long ExecutionTimeMs { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}