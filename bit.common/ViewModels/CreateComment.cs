using bit.common.Models;
using bit.common.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace bit.common.Commands
{
    public class CreateComment
    {
        [Required]
        public string PostId { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
