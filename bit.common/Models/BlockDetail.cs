using bit.common.Contracts;
using bit.common.ViewModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.Models
{
    public class BlockDetail : IMongoCommon
    {
        public Guid UId { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CountryId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerId { get; set; }
        public int BlockAxis { get; set; }
        public int BlockYxis { get; set; }
        public int BlockNumber { get; set; }
        public int TotalResidents { get; set; }
    }
}
