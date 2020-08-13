using MongoDB.Bson.Serialization.Attributes;
using System;

namespace rapide_shortener_service.Model
{
    public class URLModel : MongoModel
    {


        [BsonElement("ShortURL")]
        public string ShortURL { get; set; }

        [BsonElement("originalURL")]
        public string OriginalURL { get; set; }


    }
}