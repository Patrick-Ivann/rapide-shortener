using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace rapide_shortener_service.Model
{
    public abstract class MongoModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}

