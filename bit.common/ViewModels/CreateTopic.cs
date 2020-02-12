using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bit.common.Commands
{
    public class CreateTopic
    {
        [Required]
        public int BlockAxis { get; set; }
        [Required]
        public int BlockYxis { get; set; }
        [Required]
        public string CountryName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string CountryId { get; set; }
        public string OwnerId { get; set; }
        public Guid UserUId { get; set; }
        public int BlockNumber { get; set; }
    }
}
