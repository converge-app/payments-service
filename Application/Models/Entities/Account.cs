using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.Models.Entities
{
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public bool LiveMode { get; set; }
        public string RefreshToken { get; set; }
        public string StripePublisableKey { get; set; }
        public string StripeUserId { get; set; }
        public string Scope { get; set; }
    }
}