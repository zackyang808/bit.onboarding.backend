using bit.common.Contracts;
using bit.common.ViewModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.Models
{
    // TODO: Where do we keep role/permission related info?
    class BlockResident : IMongoCommon
    {
        public Guid UId { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string BlockId { get; set; }
        public UserViewModel Resident { get; set; }
        public bool IsDeleted { get; set; }
    }
}
