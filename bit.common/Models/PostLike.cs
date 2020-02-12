using bit.common.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.Models
{
    public class PostLike : IMongoCommon
    {
        public Guid UId { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public PostLikeType Type { get; set; }
        public bool IsDeleted { get; set; }
    }

    public enum PostLikeType
    {
        Like,
        Dislike,
    }
}
