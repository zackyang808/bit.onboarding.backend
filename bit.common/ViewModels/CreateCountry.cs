using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bit.common.Commands
{
    public class CreateCountry
    {
        [Required]
        public int BlockNumber { get; set; }
        [Required]
        public string CountryName { get; set; }
        [Required]
        public string CountryDescription { get; set; }
        [Required]
        public string Theme { get; set; }
        [Required]
        public int CountryContractUniqueId { get; set; }
        public string CountryOwner { get; set; }
        public string OwnerAddress { get; set; }
        public Guid UserUId { get; set; }
        public string UserId { get; set; }
        public int CountryIndex { get; set; }

        public string Status { get; set; }
        public string TxTran { get; set; }
    }
}
