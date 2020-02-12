using bit.common.Contracts;
using bit.common.ViewModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace bit.common.Models
{
    public class Post : IMongoCommon
    {
        public Guid UId { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string FeaturedImage { get; set; }
        public string BlockId { get; set; }
        public string CountryId { get; set; }
        public int CountryIndex { get; set; } // TODO: This is 0 on all posts so far.
        public int Likes { get; set; } // TODO: Move to settlement collection
        public int BlockNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; } // TODO: Remove
        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public PostStatus Status { get; set; }
        public UserViewModel Owner { get; set; }
        public PostType PostType { get; set; }
    }

    public enum PostStatus
    {
        Active,
        Spammed,
        InActive,
        PendingApproved,
    }

    public enum PostType
    {
        story,
        article,
    }
}
