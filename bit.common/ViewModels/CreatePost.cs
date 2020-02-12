using System;
using System.ComponentModel.DataAnnotations;

namespace bit.common.Models
{
    public class CreatePost
    {
        [Required]
        public string BlockId { get; set; }
        [Required]
        public string CountryId { get; set; }
        public int BlockNumber { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Content { get; set; }
        public string OwnerId { get; set; }
        public string OwnerWalletAddress { get; set; }
        public string[] Images { get; set; }
        public Guid UserUId { get; set; }
        public string PostType { get; set; }
    }

    public class GetPostModel
    {
        public string BlockId { get; set; }
        public int OffSet { get; set; }
    }
}