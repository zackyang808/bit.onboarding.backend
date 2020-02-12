using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace bit.common.Commands
{
    public class CreateCountryActivityRule
    {
        [Required]
        public string CountryId { get; set; }
        [Required]
        public int Story { get; set; }
        [Required]
        public int StoryLike { get; set; }
        [Required]
        public int StoryView { get; set; }
        [Required]
        public int Article { get; set; }
        [Required]
        public int ArticleLike { get; set; }
        [Required]
        public int ArticleView { get; set; }
        [Required]
        public int Comment { get; set; }
        [Required]
        public int CommentLike { get; set; }
        [Required]
        public int Share { get; set; }
        [Required]
        public int Referral { get; set; }
    }
}
