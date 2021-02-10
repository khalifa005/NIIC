using Domains.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domains
{
    public class Comment
    {
        public int Id { get; set; }
        public string  Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public AppUser Author { get; set; }
        public Activity Activity { get; set; }

        //[ForeignKey("ParentId")]
        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; }
    }
}
