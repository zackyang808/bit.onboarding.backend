using bit.common.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace bit.common.Models
{
    public class Country : IMongoCommon
    {
        public Guid UId { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string President { get; set; }
        public decimal BankBalance { get; set; }
        public int Population { get; set; }
        public string Theme { get; set; }
        public List<int> DigitalAssets { get; set; }
        public string ThemeImage { get; set; }
        public decimal EstimatedValue { get; set; }
        public decimal OriginalValue { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int CountryIndex { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerId { get; set; }
        public int BlockNumber { get; set; }
        public int CountryContractUniqueId { get; set; }
        public string Status { get; set; }
        public string TxTran { get; set; }
        public List<Block> Blocks { get; set; }
        public int TotalBlocks { get; set; }
        public string WelcomeMessage { get; set; }
    }

    [FunctionOutput]
    public class Block
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Guid UId { get; set; }
        [Parameter("address", "blockOwner", 1)]
        public string BlockOwner { get; set; }
        [Parameter("int", "axis", 2)]
        public int Axis { get; set; }
        [Parameter("int", "yxis", 3)]
        public int YAxis { get; set; }
        public int BlockNumber { get; set; }
        public string Status { get; set; }
        public string TxTran { get; set; }
    }

    public static class Status
    {
        public static string Pending => "Pending";
        public static string Completed => "Completed";
        public static string NewBlockAddedPending => "NewBlockAddedPending";
    }

    public static class TransactionStatus
    {
        public static string NotFound => "NotFound";
        public static string Success => "Success";
        public static string Fail => "Fail";
    }

    [FunctionOutput]
    public class CountryStructContract
    {
        [Parameter("string", "name", 1)]
        public string CountryName { get; set; }
        [Parameter("uint", "theme", 2)]
        public int Theme { get; set; }
        [Parameter("uint", "numberOfBlock", 3)]
        public int TotalBlocks { get; set; }
        [Parameter("address", "reservedBankAddress", 4)]
        public string ReserveBankAddress { get; set; }
    }
}
