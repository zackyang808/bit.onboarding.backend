using bit.common.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;


namespace bit.common.Models
{
    public class CountryActivityRule : IMongoCommon
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CountryId { get; set; }
        public int Story { get; set; }
        public int StoryLike { get; set; }
        public int StoryView { get; set; }
        public int Article { get; set; }
        public int ArticleLike { get; set; }
        public int ArticleView { get; set; }
        public int Comment { get; set; }
        public int CommentLike { get; set; }
        public int Share { get; set; }
        public int Referral { get; set; }
        public bool IsDeleted { get; set; }

    }
}
