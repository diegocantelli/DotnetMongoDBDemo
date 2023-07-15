using MongoDB.Bson.Serialization.Attributes;

namespace DriversAppApi.Models 
{
    public class Driver 
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; } = null!;

        [BsonElement("Name")] // Name sera o nome definido na collection do mongoDB
        public string DriverName { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string TeamId { get; set; }

        public int Number { get; set; }
    }
}