using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Osprey_Project
{
    internal class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // MongoDB Unique ID
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}