using bit.common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace bit.common.Commands
{
    public class CreateCommentActivity
    {
        [Required]
        public string CommentId { get; set; }
    }
}
