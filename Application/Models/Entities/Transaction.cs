using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.Models.Entities
{
    public class Transaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public decimal amount { get; set; }
        public string status { get; set; }
        public bool count { get; set; }
    }
}