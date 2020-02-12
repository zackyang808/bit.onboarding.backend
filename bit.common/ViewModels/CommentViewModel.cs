using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.ViewModels
{
    public class CommentViewModel
    {
        public string Id { get; set; }
        public UserViewModel Author { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool isLiked { get; set; }
        public bool isDisliked { get; set; }
    }
}
