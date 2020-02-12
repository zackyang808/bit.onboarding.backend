using System;
using bit.common.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bit.common.Models
{
    // TODO: Deprecated - unless we want to have Featured Image or similar for stories and articles.
    // Images will be stored Base64 encoded in Azure Blob Storage with the rest of Post details.
    public class PostImage : IMongoCommon
    {
        public Guid UId { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PostId { get; set; }
        public string BlockId { get; set; }
        public string ImageUrl { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}