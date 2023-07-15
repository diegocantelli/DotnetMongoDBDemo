using MongoDB.Bson.Serialization.Attributes;

namespace DriversAppApi.Models
{
    public class Team
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}