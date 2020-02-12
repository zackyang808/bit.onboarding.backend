using bit.common.Models;
using System;

namespace bit.common.ViewModels
{
    public class PostViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string FeaturedImage { get; set; }
        public string BlockId { get; set; }
        public string CountryId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public PostStatus Status { get; set; }
        public UserViewModel Owner { get; set; }
        public PostType PostType { get; set; }
        public bool isLiked { get; set; }
        public bool isDisliked { get; set; }
    }
}