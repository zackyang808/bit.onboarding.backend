using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.ViewModels
{
    public class UserViewModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string WalletAddress { get; set; }
        public string NickName { get; set; } // TODO: This must be updated if the user changes their NickName!
        public string ProfileImageUrl { get; set; }
    }
}
